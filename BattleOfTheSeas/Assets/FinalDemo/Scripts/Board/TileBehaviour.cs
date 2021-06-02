using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
public class TileBehaviour : MonoBehaviourPun
{
    public enum TileState
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

    private int _maxUntargetableRounds = 2;

    #endregion

    #region Public Variables

    public bool HasShip = false;
    
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
    public TileState State => _tileState;

    #endregion

    #region MonoBehaviourCallbacks
    void Start()
    {
        _playerManager = PlayerManager.Instance;
        _turnBasedSystem = TurnBasedSystem.Instance;

        _turnBasedSystem.OnEndTurnCallbacks += ResetState;
        
        _renderer = GetComponent<MeshRenderer>();

        _missile = transform.GetChild(0).gameObject;
    }
    
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Ship"))
    //     {
    //         _shipRef = other.gameObject;
    //         HasShip = true;
    //     }
    // }
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Ship"))
    //     {
    //         _shipRef = null;
    //         HasShip = false;
    //     }
    // }
    
    private void OnMouseDown()
    {
        //<<MAIN GAMEPLAY LOOP>>//
        if (_turnBasedSystem.State == TurnBasedSystem.GameState.IN_PROGRESS && !_turnBasedSystem.IsPLayerTurnOver)
        {
            if (_playerManager.Action != PlayerManager.ActionType.NO_ACTION)
            {
                print("Tile ID: " + _tileID);
                
                //Replicate Actions on Selected Tile
                photonView.RPC("TileAction", RpcTarget.AllBuffered);

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
        Transform iconTransform = _playerManager.TileSelector.transform;
        iconTransform.SetPositionAndRotation(transform.position, transform.rotation);
    }
    private void OnMouseExit()
    {
        Transform iconTransform = _playerManager.TileSelector.transform;
        iconTransform.SetPositionAndRotation(new Vector3(0, -100, 0), Quaternion.identity);
    }

    #endregion

    #region Public Functions
    
    // public void CheckForShip()
    // {
    //     RaycastHit hit;
    //     
    //     if (Physics.Raycast(transform.position, 
    //         transform.TransformDirection(Vector3.up), out hit,1f))
    //     {
    //         Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.red, 10f , false);
    //         
    //         GameObject objectHit = hit.collider.gameObject;
    //     
    //         if (objectHit == this.gameObject)
    //             return;
    //         
    //         if (objectHit.CompareTag("Ship"))
    //         {
    //             print(objectHit.name + " Ship Hit");
    //             _shipRef = gameObject;
    //             HasShip = true;
    //             
    //             ChangeTileColor(Color.magenta);
    //         }
    //     }
    //     else
    //     {
    //         _shipRef = null;
    //         HasShip = false;
    //             
    //         ChangeTileColor(Color.blue);
    //     }
    // }
    
    public void AttackTile()
    {
        if (_tileState == TileState.TILE_DESTROYED || _tileState == TileState.UNTARGETABLE)
            return;

        if (_missile)
            _missile.SetActive(true);

        if (HasShip)
        {
            // I don't like this but since _shipRef can change mid game it's necessary
            // Other solution is getting HealthComp after ships are locked,
            // however this would, *in theory* be more performance taxing
            if (_shipRef)
            {
                ShipHealth health = _shipRef.GetComponent<ShipHealth>();
                health.ShipHit();

                _tileState = TileState.TILE_DESTROYED;
                ChangeTileColor(Color.red);
            }
        }
        else
        {
            _tileState = TileState.TILE_DESTROYED;

            ChangeTileColor(Color.white);
        }
    }

    public void DefendTile()
    {
        if (_tileState == TileState.TILE_DESTROYED || _tileState == TileState.UNTARGETABLE)
            return;

        ChangeTileColor(Color.green);
        
        _tileState = TileState.UNTARGETABLE;
    }

    public void RestoreTile()
    {
        if (!HasShip || _tileState == TileState.CLEAR || _tileState == TileState.UNTARGETABLE)
            return;

        if (_shipRef)
        {
            ShipHealth health = _shipRef.GetComponent<ShipHealth>();

            if (health._shipHealth != 0)
            {
                health.RestoreHealth();

                ChangeTileColor(Color.blue);
                _tileState = TileState.CLEAR;
            }
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
        
        HasShip = true;
    }
    private void ChangeTileColor(Color newColor)
    {
        _renderer.material.color = newColor;
    }

    private void ResetState()
    {
        if (_tileState == TileState.UNTARGETABLE)
        {
            _maxUntargetableRounds--;

            if (_maxUntargetableRounds == 0)
            {
                ChangeTileColor(Color.blue);
                _tileState = TileState.CLEAR;
            }
        }
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