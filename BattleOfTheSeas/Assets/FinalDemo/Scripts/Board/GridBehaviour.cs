using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class GridBehaviour : MonoBehaviour
{
    #region PublicVariables

    public List<ShipBehavior> Ships => _shipBehaviors;
    public TileBehaviour[] Tiles => _tiles;

    #endregion

    #region Serialized

    [SerializeField] private GridBehaviour _otherBoard;
    [SerializeField] private List<ShipBehavior> _shipBehaviors;

    #endregion

    #region PrivateVariables

    private TileBehaviour[] _tiles;

    private TileBehaviour[,] _2DTiles;

    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        _tiles = gameObject.GetComponentsInChildren<TileBehaviour>();

        //Sets parent grid for the tiles
        for (int i = 0; i < 100; i++)
        {
            _tiles[i].TileID = i;
            _tiles[i].ParentGrid = this;
        }
        
        Make2DArray();
    }
    
    #endregion

    #region Public Functions

    //Layer Management
    public void SetTilesLayer(int layer)
    {
        foreach (var tile in _tiles)
        {
            tile.gameObject.layer = layer;
        }
    }
    public void SetShipLayer(int layer)
    {
        foreach (var ship in _shipBehaviors)
        {
            ship.gameObject.layer = layer;
        }
    }
    
    //Mid Game Functions
    public void RemoveShipFromGrid(ShipBehavior ship)
    {
        if (_otherBoard.FindShipToRemove(ship))
        {
            //Debug.Log("enemy ships now: " + _enemyGrid.Ships.Count);
            if (_otherBoard._shipBehaviors.Count <= 0)
            {
                TurnBasedSystem.Instance.State = TurnBasedSystem.GameState.LOCAL_WON;
            }
        }
        else if (FindShipToRemove(ship))
        {
            //Debug.Log("player ships now: " + _selfGrid.Ships.Count);
            if (_shipBehaviors.Count <= 0)
            {
                TurnBasedSystem.Instance.State = TurnBasedSystem.GameState.OTHER_WON;
            }
        }
    }
    public void CheckForShips()
    {
        // foreach (var tile in _tiles)
        // {
        //     tile.CheckForShip();
        // }

        foreach (var ship in _shipBehaviors)
        {
            int parentTile = ship.TileShip.TileID;
            int shipLength = ship.BoatLength;

            for (int i = 0; i < shipLength; i++)
            {
                
            }
        }
    }
    
    //Replicating interactions
    public void ReplicateTileAction(int tileID)
    {
        TileBehaviour tileToFire = FindCloneTile(tileID);

        if (tileToFire)
            PlayerManager.Instance.ManageTileActions(tileToFire);
    }
    public void ReplicateShipTransforms()
    {
        foreach (var ship in _shipBehaviors)
        {
            //Find tile which ship should be placed on
            TileBehaviour otherTile = FindCloneTile(ship.TileShip.TileID);

            //Get the transform of the ship's clone
            Transform otherShipTransf = ship.CloneShip.transform;

            if (otherTile)
            {
                Vector3 shipRotation = ship.transform.localRotation.eulerAngles;

                otherShipTransf.position = otherTile.transform.position;
                otherShipTransf.localRotation = Quaternion.Euler(shipRotation);
            }
        }
    }

    #endregion

    #region Private Functions
    private TileBehaviour FindCloneTile(int tileID)
    {
        if (!_otherBoard)
            return null;

        return Array.Find(_otherBoard.Tiles, tile => tile.TileID == tileID);
    }
    private bool FindShipToRemove(ShipBehavior otherShip)
    {
        ShipBehavior shipToRemove = _shipBehaviors.Find(ship => ship == otherShip);

        if (shipToRemove)
        {
            _shipBehaviors.Remove(shipToRemove);

            return true;
        }

        return false;
    }

    private void Make2DArray()
    {
        //Makde 2D array of tiles and sets IDs
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                _2DTiles[i,j] = _tiles[i * 10 + j];
            }
        }
    }

    #endregion
}