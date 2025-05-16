using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Photon.Realtime;
using UnityEngine;

public class RoomSetup
{
    public RoomOptions Options { get; private set; }
    public RoomPropertiesHandler PropertiesHandler { get; private set; }
    public string ID { get; private set; }
    public int MatchTimeLimit { get; set; }

    public RoomSetup()
    {
        GenerateRoomID();
        Options = new RoomOptions();
        Options.MaxPlayers = 4;
        Options.PlayerTtl = 5000;
        Options.EmptyRoomTtl = 0;
        Options.IsOpen = true;

        PropertiesHandler = new RoomPropertiesHandler();
        MatchTimeLimit = 300;
        PropertiesHandler.Time = MatchTimeLimit;
        UpdateRoomProperties();
        
    }

    public void Create(ConnectionManager con)
    {
        if(MatchTimeLimit <= 0)
            throw new Exception("Match Time Limit must be greater than 0");
        PropertiesHandler.Time = MatchTimeLimit;

        UpdateRoomProperties();
        
        con.CreateRoom(this);
    }
    
    private void UpdateRoomProperties()
    {
        Options.CustomRoomProperties = PropertiesHandler.Hashtable;
        Options.CustomRoomPropertiesForLobby = PropertiesHandler.Hashtable.GetStringKeys().ToArray();
    }
    
    private void GenerateRoomID()
    {
        string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        string macAddress = GetMacAddress();
        if(string.IsNullOrEmpty(macAddress))
        {
            macAddress = "Error";
        }

        string str = dateTime + macAddress;
        ID =  Mathf.Abs(str.GetHashCode()).ToString();
    }
    
    private static string GetMacAddress()
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (var networkInterface in networkInterfaces)
        {
            if (networkInterface.OperationalStatus != OperationalStatus.Up)
            {
                continue;
            }
            var addressBytes = networkInterface.GetPhysicalAddress().GetAddressBytes();
            if (addressBytes.Length == 6)
            {
                return string.Join(":", addressBytes.Select(b => b.ToString("X2")));
            }
        }
        return null;
    }
}
