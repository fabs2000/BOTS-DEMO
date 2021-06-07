using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Carrier : ShipBehavior
{
    private enum Coordiantes
    {
        TOP,
        BOT,
        LEFT,
        RIGHT
    }

    public override void ShipAction(TileBehavior tile)
    {
        TileBehavior[,] tiles = tile.ParentGrid.Tiles2D;
        Coordiantes coordiantes = Coordiantes.TOP;
        
        Vector2Int centerTileId = tile.TileID;
        int singleOffset = 1;
        Vector2Int tileId = new Vector2Int();
        
        //Center of the attack
        tile.AttackTile();

        //Attacks surrounding tiles
        for (int i = 0; i < 4; i++)
        {
            switch (coordiantes)
            {
                case Coordiantes.TOP:
                    tileId = new Vector2Int(centerTileId.x, centerTileId.y + singleOffset);
                    break;
                case Coordiantes.BOT:
                    tileId = new Vector2Int(centerTileId.x, centerTileId.y - singleOffset);
                    break;
                case Coordiantes.LEFT:
                    tileId = new Vector2Int(centerTileId.x + singleOffset, centerTileId.y);
                    break;
                case Coordiantes.RIGHT:
                    tileId = new Vector2Int(centerTileId.x - singleOffset, centerTileId.y);
                    break;
            }

            if (tileId.x >= 0 && tileId.x <= tiles.GetLength(0) &&
                tileId.y >= 0 && tileId.y <= tiles.GetLength(1))
            {
                tiles[tileId.x, tileId.y].AttackTile();
            }

            coordiantes++;
        }
    }
}