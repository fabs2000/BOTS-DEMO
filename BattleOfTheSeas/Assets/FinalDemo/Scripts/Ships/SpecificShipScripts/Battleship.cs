using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleship : ShipBehavior
{
    public override void ShipAction(TileBehavior tile)
    {
        // TileBehavior[,] tiles = tile.ParentGrid.Tiles2D;
        // Vector2Int protectiveAreaDimensions = new Vector2Int(3,5);
        //
        // Vector2Int startPoint = tile.TileID - 12;
        //
        // // Establishes a 5x3 Rectangle of protection
        // for (int  i = 0; i < protectiveAreaDimensions.y; i++)
        // {
        //     for (int tileId = startPoint; tileId < protectiveAreaDimensions.x + startPoint; tileId++)
        //     {
        //         if(tileId > 0 && tileId < tiles.Length)
        //             tiles[tileId].DefendTile();
        //     }
        //
        //     startPoint += 10;
        // }
    }
}
