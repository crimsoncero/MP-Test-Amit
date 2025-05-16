using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPun
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

    public void HideAllBoxButtons()
    {
        ShowPickupButton(false);
        ShowDropButton(false);
        ShowGiveButton(false);
    }

    public void ShowButtonsForState(bool isHoldingBox)
    {
        ShowPickupButton(!isHoldingBox);
        ShowDropButton(isHoldingBox);
    }
    
    public void HideEverything()
    {
      //  HideAllBoxButtons();
        ShowBoxMenu(false);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}