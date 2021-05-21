using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class ShipBehavior : MonoBehaviourPun
{

    public ShipBehavior CloneShip;
    
    private TileBehaviour _tileShip;
    public TileBehaviour TileShip
    {
        get => _tileShip;
        set => _tileShip = value;
    }

    private PlayerManager _playerManager;
    private Vector3 _startPos;

    #region MonobehaviourCallbacks
    void Start()
    {
        _startPos = transform.position;
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (_playerManager.SelectedShip == this && TurnBasedSystem.Instance.State == TurnBasedSystem.GameState.PREPARATION)
                {
                    transform.SetPositionAndRotation(_startPos, Quaternion.identity);
                }
                else
                {
                    Debug.Log("Can't reset ships, they are locked!");
                }
            }
        }
    }
    private void OnMouseDown()
    {
        if (!photonView.IsMine || !_playerManager)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            CloneShip.photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            
            _playerManager = FindObjectOfType<PlayerManager>();
        }

        if (photonView.IsMine)
        {
            if (_playerManager)
            {
                _playerManager.SetActiveShip(this);
            }
            else
            {
                Debug.Log("No player manager in scene");
            }
        }
    }

    #endregion
}