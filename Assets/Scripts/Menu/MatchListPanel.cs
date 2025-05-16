using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.Linq;
using Photon.Realtime;

public class MatchListPanel : MonoBehaviourPunCallbacks 
{
    [SerializeField] private RoomListButton _roomButtonPrefab;
    [SerializeField] private Transform _roomListContainer;
    
    private List<RoomListButton> _roomButtons = new List<RoomListButton>();
    

    public override void OnEnable()
    {
        base.OnEnable();
        ClearList();
        UpdateRoomList();
    }

    private void ClearList()
    {
        foreach (var room in _roomButtons)
        {
            Destroy(room.gameObject);
        }
        _roomButtons.Clear();
    }
    private void UpdateRoomList()
    {
        var updatedRoomList = ConnectionManager.Instance.RoomList;

        
        // First we check all the current room buttons to either update or remove them.
        for (int i = 0; i < _roomButtons.Count; i++)
        {
            var roomButton = _roomButtons[i];

            // Look if room is still in the list
            var index = updatedRoomList.FindIndex((t)=> t.Name == roomButton.RoomInfo.Name);
            if (index >= 0)
            {
                roomButton.SetRoomInfo(updatedRoomList[index]);
                // Check if room has room for more players
                if(roomButton.RoomInfo.PlayerCount < roomButton.RoomInfo.MaxPlayers)
                    _roomButtons[i].UpdateInfo();
                else
                {
                    Destroy(_roomButtons[i].gameObject);
                    _roomButtons.RemoveAt(i);
                }
                
                // Remove from the update list, as we updated it now.
                updatedRoomList.RemoveAt(i);
            }
            else // if it's not in the list it doesn't exist, so remove it.
            {
                Destroy(_roomButtons[i].gameObject);
                _roomButtons.RemoveAt(i);
            }
        }
        
        // Now we are left with only the new rooms that were added
        foreach (var newRoom in updatedRoomList)
        {
            var newRoomButton = Instantiate(_roomButtonPrefab, _roomListContainer);
            newRoomButton.SetRoomInfo(newRoom);
            _roomButtons.Add(newRoomButton);
        }
        
        
    }
    
    // Pun Callbacks
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        UpdateRoomList();
    }
}
