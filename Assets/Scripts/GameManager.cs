using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public enum MatchStateEnum
{
    NotStarted,
    Active,
    Paused,
    Ended,
}
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    // RPC Method Names
    private const string RPC_START_MATCH = "StartMatch";
    private const string RPC_END_MATCH = "EndMatch";
    private const string PLAYER_PREFAB_PATH = "Photon Prefabs\\Player";

    public event Action OnMatchEnd;
    
    public static GameManager Instance { get; private set; }
    
    public PlayerInteraction MyPlayerInteraction { get; private set; }
    public MatchStateEnum MatchState { get; private set; }
    
    public Dictionary<int, int> PlayerScores { get; private set; }
    public int MatchTimer { get; private set; } = 0;    
    private int _playerReadyCount = 0;
    private float _timerCount = 0f;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        MatchState = MatchStateEnum.NotStarted;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && MatchState == MatchStateEnum.Active)
        {
            UpdateTimer();

            if (MatchTimer <= 0)
                 photonView.RPC(RPC_END_MATCH, RpcTarget.All);  
        }
    }

    private void Start()
    {
        PhotonNetwork.Instantiate(PLAYER_PREFAB_PATH, Vector3.zero, Quaternion.identity);

        
        
        // Set up the player score.
        if (PlayerScores == null)
        {
            PlayerScores = new Dictionary<int, int>();
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if(!PlayerScores.ContainsKey(player.ActorNumber))
                    PlayerScores.Add(player.ActorNumber, 0);
            }
        }
        

        if (PhotonNetwork.IsMasterClient)
        {
            RoomPropertiesHandler propHandler = new RoomPropertiesHandler(PhotonNetwork.CurrentRoom.CustomProperties);
            MatchTimer = propHandler.Time;
        }
    }
    
    public void RegisterPlayer(PlayerInteraction player)
    {
        MyPlayerInteraction = player;
        UIManager.Instance.Init();
    }

    public void PlayerReady()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        _playerReadyCount++;
        if (_playerReadyCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            photonView.RPC(RPC_START_MATCH, RpcTarget.All);
        } 
    }

    public void UpdatePlayerScore(PlayerInteraction player)
    {
        // Only update on the master client, the clients are updated through the server.
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        PlayerScores[player.photonView.OwnerActorNr] = player.CurrentScore;
    }
    
    [PunRPC]
    private void StartMatch()
    {
        MatchState = MatchStateEnum.Active;
    }

    [PunRPC]
    private void EndMatch()
    {
        MatchState = MatchStateEnum.Ended;
        OnMatchEnd?.Invoke();
        
        if(!PhotonNetwork.IsMasterClient)
            PhotonNetwork.LeaveRoom(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(MatchTimer);
            
            foreach (KeyValuePair<int, int> playerScore in PlayerScores)
            {
                stream.SendNext(playerScore.Key);
                stream.SendNext(playerScore.Value);
            }
        }
        else
        {
            MatchTimer = (int)stream.ReceiveNext();
            
            for (int i = 0; i < PlayerScores.Count; i++)
            {
                int key = (int)stream.ReceiveNext();
                int value = (int)stream.ReceiveNext();
                
                PlayerScores[key] = value;
            }
        }

    }

    private void UpdateTimer()
    {
        _timerCount += Time.deltaTime;
        if (_timerCount > 1f)
        {
            MatchTimer--;
            _timerCount -= 1f;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (PhotonNetwork.IsMasterClient && MatchState == MatchStateEnum.Ended)
        {
            // Wait for all the clients to leave, so we check if the master client is the only one active
            int count = 0;
            foreach (var player in PhotonNetwork.PlayerList)
                count += player.IsInactive ? 0 : 1; // We do not count inactive players after the game ended.
            
            if(count == 1)
                PhotonNetwork.LeaveRoom();
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 30), "Disconnect"))
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        StartCoroutine(ReconnectCoroutine());
    }

    private IEnumerator ReconnectCoroutine()
    {
        if (!PhotonNetwork.ReconnectAndRejoin())
            yield return new WaitForSeconds(1f);
    }

    
}
