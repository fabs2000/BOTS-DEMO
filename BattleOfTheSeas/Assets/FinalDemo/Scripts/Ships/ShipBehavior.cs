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

    //Old 
    private int _boatLength;

    private bool m_HitDetect;
    private RaycastHit m_Hit;

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
        
        //Old
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
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Tile"))
    //     {
    //         TileBehavior tile = other.GetComponent<TileBehavior>();
    //
    //         if (tile)
    //         {
    //             tile.RegisterShipOnTile(gameObject);
    //         }
    //     }
    // }
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Tile"))
    //     {
    //         TileBehavior tile = other.GetComponent<TileBehavior>();
    //
    //         if (tile)
    //         {
    //             tile.UnRegisterTile();
    //         }
    //     }
    // }

    #endregion
    
    #region Public Functions

    public virtual void ShipAction(TileBehavior tile) { }

    #endregion
}