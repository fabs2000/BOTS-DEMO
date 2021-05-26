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

    #endregion

    #region MonoBehaviour Callbacks
    private void Start()
    {
        _tiles = gameObject.GetComponentsInChildren<TileBehaviour>();

        //Sets an ID for the tile
        for (int i = 0; i < 100; i++)
        {
            _tiles[i].TileID = i;
            _tiles[i].ParentGrid = this;
        }
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
    
    private TileBehaviour FindCloneTile(int tileID)
    {
        return Array.Find(_otherBoard.Tiles,tile => tile.TileID == tileID);
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

    #endregion
}
