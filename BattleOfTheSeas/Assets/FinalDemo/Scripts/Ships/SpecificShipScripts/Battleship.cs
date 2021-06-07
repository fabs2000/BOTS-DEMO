using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleship : ShipBehavior
{
    public override void ShipAction(TileBehavior tile)
    {
        TileBehavior[,] tiles = tile.ParentGrid.Tiles2D;
        Vector2Int protectiveAreaDimensions = new Vector2Int(5,3);
        
        Vector2Int startPoint = new Vector2Int(tile.TileID.x - 2, tile.TileID.y - 1);
        Vector2Int tileId = new Vector2Int();
        
        // Establishes a 5x3 Rectangle of protection
        for (int i = startPoint.y; i < startPoint.y + protectiveAreaDimensions.y; ++i)
        {
            for (int j = startPoint.x; j < startPoint.x + protectiveAreaDimensions.x; ++j)
            {
                tileId = new Vector2Int(j, i);

                if (tileId.x >= 0 && tileId.x <= tiles.GetLength(0) &&
                    tileId.y >= 0 && tileId.y <= tiles.GetLength(1))
                {
                    print(tileId);
                    tiles[tileId.x, tileId.y].DefendTile();
                }
            }
        }
    }
}
