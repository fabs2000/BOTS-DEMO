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


    public override void ShipAction(TileBehaviour tile)
    {
        print("Air Attack");

        TileBehaviour[] tiles = tile.ParentGrid.Tiles;
        Coordiantes coordiantes = Coordiantes.TOP;


        int centerTileId = tile.TileID;
        int fullOffset = 10, singleOffset = 1;
        int tileId = 0;

        //Center of the attack
        tile.AttackTile();

        //Attacks surrounding tiles
        for (int i = 0; i < 4; i++)
        {
            switch (coordiantes)
            {
                case Coordiantes.TOP:
                    tileId = centerTileId - fullOffset;
                    break;
                case Coordiantes.BOT:
                    tileId = centerTileId + fullOffset;
                    break;
                case Coordiantes.LEFT:
                    tileId = centerTileId - singleOffset;
                    break;
                case Coordiantes.RIGHT:
                    tileId = centerTileId + singleOffset;
                    break;
            }

            if (tileId < 0)
                tileId += tiles.Length;
            else if (tileId > tiles.Length)
                tileId -= tiles.Length;
            
            coordiantes++;
            
            tiles[tileId].AttackTile();
        }
    }
}