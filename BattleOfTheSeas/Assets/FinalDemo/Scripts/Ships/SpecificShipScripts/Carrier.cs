using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Carrier : ShipBehavior
{
    public override void ShipAction(TileBehavior tile)
    {
        TileBehavior[,] tiles = tile.ParentGrid.Tiles2D;

        Vector2Int centerTileId = tile.TileID;
        int singleOffset = 1;

        //Center of the attack
        tile.AttackTile();

        Vector2Int[] ids = { 
            //TOP
            new Vector2Int(centerTileId.x, centerTileId.y + singleOffset), 
            //BOT
            new Vector2Int(centerTileId.x, centerTileId.y - singleOffset), 
            //LEFT
            new Vector2Int(centerTileId.x + singleOffset, centerTileId.y),
            //RIGHT
            new Vector2Int(centerTileId.x - singleOffset, centerTileId.y) 
        };

        //Attacks surrounding tiles
        foreach (var tileId in ids)
        {
            if (tileId.x >= 0 && tileId.x < tiles.GetLength(0) &&
                tileId.y >= 0 && tileId.y < tiles.GetLength(1))
            {
                tiles[tileId.x, tileId.y].AttackTile();
            }
        }
    }
}