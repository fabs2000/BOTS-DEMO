using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class ShipBehavior : MonoBehaviourPun
{
    public enum ShipType
    {
        CARRIER = 0,
        SUBMARINE = 1,
        DESTROYER = 2,
        BATTLESHIP = 3,
        CRUISER = 4
    }
    
    #region Private Variables
    
    private TileBehavior _tileShip;
    private PlayerManager _playerManager;
    private Vector3 _startPos;

    //New 
    private TileBehavior[] _tiles;
    private int _boatLength;

    #endregion

    #region Public Variables

    public ShipBehavior CloneShip;
    public ShipType ShipClass;
    public int ActionCooldown = 0;
    
    public TileBehavior TileShip
    {
        get => _tileShip;
        set => _tileShip = value;
    }

    public int BoatLength => _boatLength;

    #endregion

    #region MonobehaviourCallbacks
    void Start()
    {
        _playerManager = PlayerManager.Instance;
        _startPos = transform.position;

        //New
        switch (ShipClass)
        {
            case ShipType.CARRIER:
                _boatLength = 5;
                break;
            
            case ShipType.BATTLESHIP:
                _boatLength = 4;
                break;
            
            case ShipType.SUBMARINE:
                _boatLength = 3;
                break;
            
            case ShipType.CRUISER:
                _boatLength = 3;
                break;
            
            case ShipType.DESTROYER:
                _boatLength = 2;
                break;
        }
    }
    
    // private void Update()
    // {
    //     if (photonView.IsMine)
    //     {
    //         if (Input.GetMouseButtonDown(1))
    //         {
    //             if (_playerManager.SelectedShip == this 
    //                 && TurnBasedSystem.Instance.State == TurnBasedSystem.GameState.PREPARATION)
    //             {
    //                 transform.SetPositionAndRotation(_startPos, Quaternion.identity);
    //             }
    //             else
    //             {
    //                 Debug.Log("Can't reset ships, they are locked!");
    //             }
    //         }
    //     }
    // }
    
    private void OnMouseDown()
    {
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            CloneShip.photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }

        if (photonView.IsMine)
        {
            if (_playerManager)
            {
                _playerManager.SelectedShip = this;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            TileBehavior tile = other.GetComponent<TileBehavior>();

            if (tile)
            {
                tile.RegisterShipOnTile(gameObject);
            }
        }
    }

    #endregion
    
    #region Public Functions

    public virtual void ShipAction(TileBehavior tile) { }

    #endregion
}