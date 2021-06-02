using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    #region PublicVariables

    public List<ShipBehavior> Ships => _shipBehaviors;
    public TileBehavior[] Tiles => _tiles;
    public TileBehavior[,] Tiles2D => _2DTiles;

    #endregion

    #region Serialized

    [SerializeField] private GridBehaviour _otherBoard;
    [SerializeField] private List<ShipBehavior> _shipBehaviors;

    #endregion

    #region PrivateVariables

    private TileBehavior[] _tiles;
    private TileBehavior[,] _2DTiles = new TileBehavior[10, 10];

    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        TurnBasedSystem.Instance.OnBeginPreparationCallbacks += SetTilesClones;
        
        _tiles = gameObject.GetComponentsInChildren<TileBehavior>();

        //Sets parent grid for the tiles
        for (int i = 0; i < 100; i++)
        {
            _tiles[i].ParentGrid = this;
        }

        Make2DArray();
    }

    #endregion

    #region Public Functions

    //Layer Management
    public void SetTilesLayer(int layer)
    {
        foreach (var tile in _2DTiles)
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
        foreach (var ship in _shipBehaviors)
        {
            if (!ship.TileShip)
                print("Stop here");

            Vector2Int parentTileID = ship.TileShip.TileID;

            int shipLength = ship.BoatLength;
            
            Vector3 rotation = ship.transform.rotation.eulerAngles;
            Vector2Int direction = new Vector2Int(1, 0);

            int rotY = (int) rotation.y;
            
            if (rotY % 180 == 0)
            {
                if(rotY == 0)
                    direction = new Vector2Int(0, 1);
                else
                    direction = new Vector2Int(0, -1);
            }
            else if (rotY % 180 == 90)
            {
                if(rotY == 90)
                    direction = new Vector2Int(-1, 0);
                else
                    direction = new Vector2Int(1, 0);
            }
            
            
            if (!PhotonNetwork.IsMasterClient)
                direction *= -1;
            
            for (int i = 1; i < shipLength; i++)
            {
                UnityEngine.Vector2Int newID = parentTileID + direction;
                
                _2DTiles[newID.x, newID.y].RegisterShipOnTile();
            }
        }
    }

    //Replicating interactions
    public void ReplicateTileAction(TileBehavior tile)
    {
        TileBehavior tileToFire = FindCloneTile(tile.TileID);

        if (tileToFire)
            PlayerManager.Instance.ManageTileActions(tileToFire);
    }
    public void ReplicateShipTransforms()
    {
        foreach (var ship in _shipBehaviors)
        {
            //Get tile which ship should be placed on
            TileBehavior otherTile = FindCloneTile(ship.TileShip.TileID);

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
    //Somehow needed to replicate transforms properly
    private TileBehavior FindCloneTile(Vector2Int tileID)
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
        //Make 2D array of tiles and sets IDs
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                _2DTiles[i, j] = _tiles[i * 10 + j];
                _2DTiles[i, j].TileID = new Vector2Int(i, j);
            }
        }
    }

    private IEnumerator LoopGrid()
    {
        UnityEngine.Vector2Int mockID1 = new UnityEngine.Vector2Int(3, 3);
        Vector2 mockID2 = new Vector2(2, 8);

        for (int i = 0; i < 5; i++)
        {
            _2DTiles[mockID1.x + i, mockID1.y].RegisterShipOnTile();

            yield return new WaitForSeconds(0.2f);
        }


        for (int i = 0; i < 3; i++)
        {
            _2DTiles[mockID1.x, mockID1.y + i].RegisterShipOnTile();

            yield return new WaitForSeconds(0.2f);
        }
    }
    private void SetTilesClones()
    {
        foreach (var tile in _tiles)
        {
            tile.CloneTile = _otherBoard.Tiles2D[tile.TileID.x, tile.TileID.y];
        }
    }

    #endregion
}