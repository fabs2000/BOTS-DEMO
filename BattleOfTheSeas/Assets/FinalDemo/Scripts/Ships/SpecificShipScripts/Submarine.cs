using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : ShipBehavior
{
    public override void ShipAction(TileBehavior tile)
    {
        StartCoroutine(RowAttack(tile));
    }

    private IEnumerator RowAttack(TileBehavior tile)
    {
        TileBehavior[,] tiles = tile.ParentGrid.Tiles2D;
        Vector2Int tileID = tile.TileID;

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            yield return new WaitForSeconds(0.1f);
            tiles[tileID.x, i].AttackTile();
        }
    }
}
