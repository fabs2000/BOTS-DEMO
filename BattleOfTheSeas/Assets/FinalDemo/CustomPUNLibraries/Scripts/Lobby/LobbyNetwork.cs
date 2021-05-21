using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

/// <summary>
/// This class works as the "Launcher" for the game, manages everything lobby related
/// </summary>
public class LobbyNetwork : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _roomNameInputField;

    [Space(10)]
    [SerializeField] private byte _maxPlayers;

    [Space(10)] 
    [SerializeField] private string _levelToLoad;

    private RoomOptions _defaultRoomOptions;
    private List<string> _roomNames = new List<string>();
    private List<GameObject> _roomList = new List<GameObject>();

    private GameObject _roomDisplayTemplate;

    #region MonoBehaviourCallbacks
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
    }

    private void Start()
    {
        _roomDisplayTemplate = Resources.Load<GameObject>("RoomPrefab");
        
        _defaultRoomOptions = new RoomOptions {MaxPlayers = _maxPlayers, PlayerTtl = 10000};
    }

    #endregion

    #region Custom
    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            print("Connecting to Master");
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            print("Disconnecting from Master");
            PhotonNetwork.Disconnect();
        }
    }
    public void QuickMatch()
    {
        //Searches for a random room to join
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InLobby)
                PhotonNetwork.LeaveLobby();

            PhotonNetwork.JoinRandomRoom();
        }
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(_roomNameInputField.text))
        {
            Debug.LogWarning("No name given for room, try again!");
            return;
        }

        PhotonNetwork.JoinOrCreateRoom(_roomNameInputField.text, _defaultRoomOptions, PhotonNetwork.CurrentLobby);
    }
    public void ShowAllRooms(GameObject layoutGroup)
    {        
        foreach (var roomName in _roomNames)
        {
            GameObject roomDisplay = Instantiate(_roomDisplayTemplate, layoutGroup.transform);
            roomDisplay.GetComponentInChildren<TextMeshProUGUI>().text = roomName;

            _roomList.Add(roomDisplay);
        }
    }
    public void DestroyRooms()
    {
        foreach (var room in _roomList)
        {
            Destroy(room);
        }
        
        _roomList.Clear();
    }
    
    #endregion

    #region SuccessStates
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        print("OnConnectedToMaster() was called by PUN");
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("Title");
        print("OnJoinedLobby() was called by PUN, ");
    }
    public override void OnJoinedRoom()
    {
        //Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
        PhotonNetwork.LoadLevel(_levelToLoad);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Debug.Log("Room listing was updated");

        if (roomList.Count == 0)
            return;

        //Updates the list of rooms kept by each client every time a new one is created
        foreach (var roomInfo in roomList)
        {
            if (roomInfo.PlayerCount != roomInfo.MaxPlayers)
            {
                _roomNames.Add(roomInfo.Name);
            }
        }
    }

    #endregion

    #region FailStates

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //Debug.LogError("Failed to create a room with message " + message);
    }

    public override void OnLeftLobby()
    {
        //print("PUN BotS/Lobby: OnLeftLobby() was called by PUN, " +
        //      "Players connected to Lobby: " + PhotonNetwork.CurrentLobby);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log("PUN BotS/Lobby: OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        
        string randomName = "Room#" + Random.Range(1, 9999);
        PhotonNetwork.JoinOrCreateRoom(randomName, _defaultRoomOptions, PhotonNetwork.CurrentLobby);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join a room with message " + message);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    #endregion
}