using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TileBehaviour : MonoBehaviourPun
{
    private PlayerManager _playerManager;
    private TurnBasedSystem _turnBasedSystem;

    private MeshRenderer _renderer;
    private GameObject _shipRef, _missile;

    private GridBehaviour _parentGrid;

    public GridBehaviour ParentGrid
    {
        set => _parentGrid = value;
    }

    private bool _tileShot = false;
    private int _tileID;

    public int TileID
    {
        get => _tileID;
        set => _tileID = value;
    }

    #region MonoBehaviourCallbacks
    void Start()
    {
        _turnBasedSystem = TurnBasedSystem.Instance;

        _renderer = GetComponent<MeshRenderer>();

        _missile = transform.GetChild(0).gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship"))
            _shipRef = other.gameObject;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ship"))
            _shipRef = null;
    }

    #endregion

    #region RPC
    
    #endregion

    #region Public Functions

    public void BasicAttack()
    {
        if (_missile)
            _missile.SetActive(true);

        if (_shipRef)
        {
            _shipRef.GetComponent<ShipHealth>().ShipHit();
            ChangeTileColor(ColorToVec3(Color.red));
        }
        else
        {
            ChangeTileColor(ColorToVec3(Color.white));
        }
        
        _tileShot = true;
    }

    #endregion
    
    #region Private Functions

    private void PlaceShip()
    {
        Vector3 tilePos = transform.position;
        ShipBehavior ship = _playerManager.SelectedShip;

        //Set ship position
        ship.TileShip = this;
        ship.transform.position = tilePos;
    }

    private void ChangeTileColor(Vector3 newColor)
    {
        Vector4 colorChange = new Vector4(newColor.x, newColor.y, newColor.z, 1f);
        _renderer.material.color = colorChange;
    }
    
    private Vector3 ColorToVec3(Color color)
    {
        return new Vector3(color.r, color.g, color.b);
    }

    #endregion

    #region MouseInteractions
    private void OnMouseDown()
    {
        if (_tileShot)
            return;

        // This is messy but it is due to the "PlayerManager" object not being initialized for some reason.
        if (!_playerManager)
            _playerManager = FindObjectOfType<PlayerManager>();


        //<<MAIN GAMEPLAY LOOP>>//
        if (_turnBasedSystem.State == TurnBasedSystem.GameState.IN_PROGRESS &&
            !_turnBasedSystem.IsPLayerTurnOver)
        {
            //Player Actions on Selected Tile
            _playerManager.ManageTileActions(_tileID);
            _parentGrid.ReplicateTileAction(_tileID);

            //Swap player turn
            _turnBasedSystem.EndPlayerTurn();
        }
        else if (_turnBasedSystem.State == TurnBasedSystem.GameState.PREPARATION)
        {
            if (_playerManager.SelectedShip)
                PlaceShip();
            else
                Debug.Log("No Ship selected!");
        }
    }

    private void OnMouseEnter()
    {
        if (!_tileShot && _turnBasedSystem.State != TurnBasedSystem.GameState.OTHER_WON)
        {
            ChangeTileColor(ColorToVec3(Color.yellow));
        }
    }

    private void OnMouseExit()
    {
        if (!_tileShot && _turnBasedSystem.State != TurnBasedSystem.GameState.OTHER_WON)
        {
            ChangeTileColor(ColorToVec3(Color.blue));
        }
    }

    #endregion
}