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
        GameObject rowParent = tile.transform.parent.gameObject;
        TileBehavior[] tileRow = rowParent.GetComponentsInChildren<TileBehavior>();

        foreach (var tileBehaviour in tileRow)
        {
            yield return new WaitForSeconds(0.1f);
            tileBehaviour.AttackTile();
        }
    }
}
