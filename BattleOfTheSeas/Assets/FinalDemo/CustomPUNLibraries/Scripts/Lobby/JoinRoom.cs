using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class JoinRoom : MonoBehaviour
{
    private string _roomName;

    public void Join()
    {
        _roomName = GetComponentInChildren<TextMeshProUGUI>().text;
        PhotonNetwork.JoinRoom(_roomName);
    }
}
