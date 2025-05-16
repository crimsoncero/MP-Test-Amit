using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerCountText;
    [SerializeField] private Button _button;
    private RoomInfo _roomInfo;
    public RoomInfo RoomInfo => _roomInfo;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        UpdateInfo();
    }

    public void JoinRoom()
    {
        ConnectionManager.Instance.JoinRoom(_roomInfo);
    }
    public void UpdateInfo()
    {
        _playerCountText.text = _roomInfo.PlayerCount.ToString() + "/" + _roomInfo.MaxPlayers.ToString();
        ValidateButton();
    }
    private void ValidateButton()
    {
        _button.interactable = _roomInfo.PlayerCount < _roomInfo.MaxPlayers;
    }
    
    
}
