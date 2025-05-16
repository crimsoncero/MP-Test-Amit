using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateMatchPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _timeInput;
    [SerializeField] private Button _createMatchButton;
    private RoomSetup _roomSetup;
    private void Start()
    {
        _roomSetup = new RoomSetup();
        _createMatchButton.interactable = false;
    }

    public void CheckTimeValidity()
    {
        if (int.TryParse(_timeInput.text, out int time) && time > 0)
            _createMatchButton.interactable = true;
        else
            _createMatchButton.interactable = false;
    }
    
    public void CreateMatch()
    {
        _roomSetup.MatchTimeLimit = int.Parse(_timeInput.text);
        _roomSetup.Create(ConnectionManager.Instance);
    }
    
    
}
