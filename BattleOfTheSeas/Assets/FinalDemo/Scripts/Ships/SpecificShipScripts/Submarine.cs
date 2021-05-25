using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : ShipBehavior
{
    public override void ShipAction(TileBehaviour tile)
    {
        print("Submarine");

        StartCoroutine(RowAttack(tile));
    }

    private IEnumerator RowAttack(TileBehaviour tile)
    {
        GameObject rowParent = tile.transform.parent.gameObject;
        TileBehaviour[] tileRow = rowParent.GetComponentsInChildren<TileBehaviour>();

        foreach (var tileBehaviour in tileRow)
        {
            yield return new WaitForSeconds(0.5f);
            tileBehaviour.BasicAttack();
        }
    }
}
