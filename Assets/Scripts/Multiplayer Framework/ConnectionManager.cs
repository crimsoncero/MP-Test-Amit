using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    private static ConnectionManager _instance;
    public static ConnectionManager Instance => _instance;

    public List<RoomInfo> RoomList { get; private set; }
    
    
    public bool IsConnected => PhotonNetwork.IsConnectedAndReady;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            RoomList = new List<RoomInfo>();
        }
        
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom(RoomSetup setup)
    {
        PhotonNetwork.CreateRoom(setup.ID, setup.Options, TypedLobby.Default);
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
    }
    
    private void SettingsSetup()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    // Photon Callbacks

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log($"{PhotonNetwork.NickName} Connected to Master");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        SettingsSetup();
        
        Debug.Log($"{PhotonNetwork.NickName} Joined Lobby");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"Joined Room Successfuly, Room Name: {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnRoomListUpdate(List<RoomInfo> newRoomList)
    {
        base.OnRoomListUpdate(newRoomList);
        foreach(var room in newRoomList)
        {
            // room exists in room list, hence was either updated or removed.
            if (RoomList.Contains(room))
            {
                if (room.RemovedFromList)
                    RoomList.Remove(room);
                else
                    RoomList[RoomList.FindIndex((r) => r.Name == room.Name)] = room;

            }
            else // New room to add to the list.
            {
                RoomList.Add(room);
            }
        }
    }
}
