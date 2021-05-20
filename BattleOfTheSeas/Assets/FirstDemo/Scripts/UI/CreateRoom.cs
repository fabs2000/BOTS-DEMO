using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _roomName;

    public void OnClick_CreateRoom()
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
}
