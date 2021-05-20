using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

/// <summary>
/// This class works as the "Launcher" for the game, manages everything lobby related
/// </summary>

public class LobbyNetwork_DEPRECTATED : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _roomName;
    
    private string _gameVersion = "1";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    #region Custom

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
        }
    }
    
    public void QuickMatch()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InLobby)
                PhotonNetwork.LeaveLobby();
            
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void CreateGame()
    {
        if (!_roomName.text.Equals(""))
        {
            if (PhotonNetwork.InLobby)
                PhotonNetwork.LeaveLobby();
            
            RoomOptions options = new RoomOptions {MaxPlayers = 2, PlayerTtl = 10000};
            PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, PhotonNetwork.CurrentLobby);
        }
        else
        {
            Debug.LogWarning("No name given for room, try again!");
        }
    }
    
    public void ShowAllRooms()
    {
        //TODO: Implement browser functionality
    }

    #endregion
    
    #region SuccessStates
    public override void OnConnectedToMaster()
    {
        print("PUN BotS/Lobby: OnConnectedToMaster() was called by PUN");
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }
    
    public override void OnJoinedLobby()
    {
        print("PUN BotS/Lobby: OnJoinedLobby() was called by PUN, ");
       
        print("Player: " + PhotonNetwork.NickName + " just joined lobby");
        
        print("Players connected to Lobby: " + PhotonNetwork.CountOfPlayers);
    }
    public override void OnCreatedRoom()
    {
        print("Room created successfully with name: " + _roomName.text);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("PUN BotS/Lobby: OnJoinedRoom() called by PUN. Now this client is in a room.");
        
        PhotonNetwork.LoadLevel("Battle Scene");
    }

    #endregion

    #region FailStates

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN BotS/Lobby: OnDisconnected() was called by PUN with reason {0}", cause);
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to create a room with message " + message);
    }

    public override void OnLeftLobby()
    {
        print("PUN BotS/Lobby: OnLeftLobby() was called by PUN, " +
            "Players connected to Lobby: " + PhotonNetwork.CurrentLobby);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN BotS/Lobby: OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        
        if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
        
        string randomName = "Room#" + Random.Range(1000, 9999);
        PhotonNetwork.JoinOrCreateRoom(randomName, new RoomOptions{MaxPlayers = 2}, PhotonNetwork.CurrentLobby);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join a room with message " + message);
    }

    #endregion
}
