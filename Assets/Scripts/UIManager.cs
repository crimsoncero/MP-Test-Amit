using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject boxMenuPanel;
    
    [Header("GUI")]
    [SerializeField] private Button pickupButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button giveButton;
    
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    [Header("Leaderboard")] 
    [SerializeField] private Transform leaderboardPanel;
    [SerializeField] private Transform scoreBoard;
    [SerializeField] private PlayerScoreElement scoreElementPrefab;
    [SerializeField] private Button returnToMenuButton;

    [Header("Disconnect")] 
    [SerializeField] private Transform disconnectPanel;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        UpdateTimer();
    }

    public void Init()
    {
        var myPlayerInteraction = GameManager.Instance.MyPlayerInteraction;
        GameManager.Instance.OnMatchEnd += ShowLeaderboard;
        pickupButton.onClick.AddListener(myPlayerInteraction.TryPickup);
        dropButton.onClick.AddListener(myPlayerInteraction.TryDrop);
        giveButton.onClick.AddListener(myPlayerInteraction.TryGive);
        returnToMenuButton.onClick.AddListener(ReturnToMenu);
        returnToMenuButton.interactable = false;
    }

    private void UpdateTimer()
    {
        var timeSpan = TimeSpan.FromSeconds(GameManager.Instance.MatchTimer);
        timeText.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
    
    private void ShowPickupButton(bool show)
    {
        pickupButton.gameObject.SetActive(show);
    }

    private void ShowDropButton(bool show)
    {
        dropButton.gameObject.SetActive(show);
    }

    public void ShowGiveButton(bool show)
    {
        giveButton.gameObject.SetActive(show);
    }

    private void ShowLeaderboard()
    {
        foreach (var playerScore in GameManager.Instance.PlayerScores)
        {
            PlayerScoreElement e = Instantiate(scoreElementPrefab, scoreBoard);
            e.Init($"P{playerScore.Key}", playerScore.Value);
        }
        leaderboardPanel.gameObject.SetActive(true);
    }
    
    public void ShowBoxMenu(bool show)
    {
        boxMenuPanel.SetActive(show);
    }
    public void ShowButtonsForState(bool isHoldingBox)
    {
        ShowPickupButton(!isHoldingBox);
        ShowDropButton(isHoldingBox);
    }
    
    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void ReturnToMenu()
    {
        if(PhotonNetwork.InRoom == false)
            SceneLoader.LoadMenuSceneAsync();
    }
    
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        returnToMenuButton.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        disconnectPanel.gameObject.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        disconnectPanel.gameObject.SetActive(false);
    }
}