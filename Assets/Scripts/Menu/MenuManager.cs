using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MenuManager : MonoBehaviourPunCallbacks
{
    private static MenuManager _instance;
    public static MenuManager Instance => _instance;

    [SerializeField] private Transform _menuPanel;
    [SerializeField] private CreateMatchPanel _createMatchPanel;
    [SerializeField] private WaitingRoomPanel _waitingRoomPanel;
    [SerializeField] private MatchListPanel _matchListPanel;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        
        CloseAllPanels();
    }

    public void OpenCreateMatchPanel()
    {
        CloseAllPanels();
        _createMatchPanel.gameObject.SetActive(true);
    }

    public void OpenWaitingRoomPanel()
    {
        CloseAllPanels();
        _waitingRoomPanel.gameObject.SetActive(true);
    }

    public void OpenMenuPanel()
    {
        CloseAllPanels();
        _menuPanel.gameObject.SetActive(true);
    }

    public void OpenMatchListPanel()
    {
        CloseAllPanels();
        _matchListPanel.gameObject.SetActive(true);
    }
    public void CloseAllPanels()
    {
        _createMatchPanel.gameObject.SetActive(false);
        _waitingRoomPanel.gameObject.SetActive(false);
        _menuPanel.gameObject.SetActive(false);
        _matchListPanel.gameObject.SetActive(false);
    }
    
    // Pun callbacks
    public override void OnJoinedLobby()
    {
        OpenMenuPanel();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        OpenWaitingRoomPanel();
    }
}
