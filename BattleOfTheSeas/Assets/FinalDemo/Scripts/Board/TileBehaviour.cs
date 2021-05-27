using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TileBehaviour : MonoBehaviourPun
{
    private enum TileState
    {
        UNTARGETABLE,
        TILE_DESTROYED,
        CLEAR
    }

    #region PrivateVariables

    private PlayerManager _playerManager;
    private TurnBasedSystem _turnBasedSystem;

    private MeshRenderer _renderer;
    private GameObject _shipRef, _missile;

    private GridBehaviour _parentGrid;

    private TileState _tileState = TileState.CLEAR;
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
        if (_tileState == TileState.TILE_DESTROYED)
            return;

        //<<MAIN GAMEPLAY LOOP>>//
        if (_turnBasedSystem.State == TurnBasedSystem.GameState.IN_PROGRESS && !_turnBasedSystem.IsPLayerTurnOver)
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
        // if ((_tileState != TileState.TILE_DESTROYED || _tileState != TileState.UNTARGETABLE)
        //     
        //     && _turnBasedSystem.State != TurnBasedSystem.GameState.OTHER_WON)
        // {
        //     ChangeTileColor(Color.yellow);
        // }
        
        //TODO: Spawn icon on tile being hovered
        

    }
    private void OnMouseExit()
    {
        // if ((_tileState != TileState.TILE_DESTROYED || _tileState != TileState.UNTARGETABLE)
        //     && _turnBasedSystem.State != TurnBasedSystem.GameState.OTHER_WON)
        // {
        //     ChangeTileColor(Color.blue);
        // }
    }

    #endregion

    #region Public Functions

    public void AttackTile()
    {
        if (_tileState == TileState.TILE_DESTROYED )
            return;

        if (_missile)
            _missile.SetActive(true);

        if (_shipRef)
        {
            // I don't like this but since _shipRef can change mid game it's necessary
            // Other solution is getting HealthComp after ships are locked,
            // however this would, *in theory* be more performance taxing
            ShipHealth health = _shipRef.GetComponent<ShipHealth>();


            health.ShipHit();
            _tileState = TileState.TILE_DESTROYED;
            ChangeTileColor(Color.red);
        }
        else
        {
            _tileState = TileState.TILE_DESTROYED;

            ChangeTileColor(Color.white);
        }
    }

    public void DefendTile()
    {
        switch (_tileState)
        {
            case TileState.CLEAR:
                return;
            
            case TileState.UNTARGETABLE:
                
                
                
                break;
            
            case TileState.TILE_DESTROYED:
                ShipHealth health = _shipRef.GetComponent<ShipHealth>();
                health.RestoreHealth();
                
                ChangeTileColor(Color.blue);

                _tileState = TileState.CLEAR;
                break;
        }
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