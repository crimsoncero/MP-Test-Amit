using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingRoomPanel : MonoBehaviourPunCallbacks
{
    
    [SerializeField] private TMP_Text _playerCountText;
    [SerializeField] private TMP_Text _matchTimeText;
    [SerializeField] private Button _startMatchButton;

    [SerializeField] private int _minimumPlayerCount = 1;

    public override void OnEnable()
    {
        base.OnEnable();
        
        if (!PhotonNetwork.InRoom) return;
        
        UpdateRoomInfo();
        CheckIfAbleToStartMatch();
        
        // Not show the start match button if not the master client
        _startMatchButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartMatch()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.PlayerTtl = -1;
        SceneLoader.LoadGameSceneAsync();
        
    }

    private void UpdateRoomInfo()
    {
        _playerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        var roomProps = new RoomPropertiesHandler(PhotonNetwork.CurrentRoom.CustomProperties);
        var timeSpan = TimeSpan.FromSeconds(roomProps.Time);
        _matchTimeText.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    private void CheckIfAbleToStartMatch()
    {
        _startMatchButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount >= _minimumPlayerCount;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        CheckIfAbleToStartMatch();
        UpdateRoomInfo();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        CheckIfAbleToStartMatch();
        UpdateRoomInfo();
    }
    
    
}
