using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TileBehaviour : MonoBehaviourPun
{
    #region PrivateVariables

    private PlayerManager _playerManager;
    private TurnBasedSystem _turnBasedSystem;

    private MeshRenderer _renderer;
    private GameObject _shipRef, _missile;

    private GridBehaviour _parentGrid;
    
    private bool _tileShot = false;
    private int _tileID;
    
    #endregion

    #region Public Variables
    
    public GridBehaviour ParentGrid
    {
        get => _parentGrid;
        set => _parentGrid = value;
    }
    public int TileID
    {
        get => _tileID;
        set => _tileID = value;
    }
    
    #endregion
    
    #region MonoBehaviourCallbacks
    void Start()
    {
        _playerManager = PlayerManager.Instance;
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
    private void OnMouseDown()
    {
        if (_tileShot)
            return;

        //<<MAIN GAMEPLAY LOOP>>//
        if (_turnBasedSystem.State == TurnBasedSystem.GameState.IN_PROGRESS &&  !_turnBasedSystem.IsPLayerTurnOver)
        {
            if (_playerManager.Action != PlayerManager.ActionType.NO_ACTION)
            {
                //Player Actions on Selected Tile
                photonView.RPC("TileAction", RpcTarget.All);

                //Swap player turn
                _turnBasedSystem.EndPlayerTurn();
            }
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
            ChangeTileColor(Color.yellow);
        }
    }
    private void OnMouseExit()
    {
        if (!_tileShot && _turnBasedSystem.State != TurnBasedSystem.GameState.OTHER_WON)
        {
            ChangeTileColor(Color.blue);
        }
    }
    
    #endregion

    #region Public Functions
    public void BasicAttack()
    {
        if(_tileShot)
            return;

        if (_missile)
            _missile.SetActive(true);

        if (_shipRef)
        {
            // I don't like this but since _shipRef can change mid game it's necessary :<
            // Other solution is getting HealthComp after ships are locked,
            // however this would, *in theory* be more performance taxing
            
            _shipRef.GetComponent<ShipHealth>().ShipHit();
            ChangeTileColor(Color.red);
        }
        else
        {
            ChangeTileColor(Color.white);
        }
        
        _tileShot = true;
    }

    public void TileDefended()
    {
        
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
    private void ChangeTileColor(Color newColor)
    {
        _renderer.material.color = newColor;
    }
    
    #endregion

    #region RPC
    
    [PunRPC]
    public void TileAction()
    {
        if (_playerManager)
        {
            _playerManager.ManageTileActions(this);
            _parentGrid.ReplicateTileAction(_tileID);
        }
    }
    
    #endregion
}