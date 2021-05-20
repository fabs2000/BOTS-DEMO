using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GridBehaviour : MonoBehaviour
{
    #region Serialized

    [SerializeField] private GridBehaviour _otherBoard;

    public List<GameObject> Ships;
    
    [SerializeField] private List<ShipBehavior> _shipBehaviors;
    
    #endregion

    #region Privates

    private TileBehaviour[] _tiles;
    public TileBehaviour[] Tiles => _tiles;

    #endregion
    
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
    
    //Adjustments
    public void SetTilesLayer(int layer)
    {
        foreach (var tile in _tiles)
        {
            tile.gameObject.layer = layer;
        }
    }
    
    public void SetShipLayer(int layer)
    {
        foreach (var ship in Ships)
        {
            ship.layer = layer;
        }
    }
    
    private TileBehaviour FindCloneTile(int tileID)
    {
        return Array.Find(_otherBoard.Tiles,tile => tile.TileID == tileID);
    }

    //Mid Game Functions
    public bool FindShipToRemove(GameObject otherShip)
    {
        GameObject shipToRemove = null;
        shipToRemove = Ships.Find(ship => ship == otherShip); 
        
        if (shipToRemove)
        {
            Ships.Remove(shipToRemove);
            return true;
        }
        
        return false;
    }

    //Replicating interactions
    public void ReplicateFire(int tileID)
    {
        TileBehaviour tileToFire = FindCloneTile(tileID);
        
        if(tileToFire)
            tileToFire.FireOnTile();
    }

    public void ReplicateShips()
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


}
