using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : ShipBehavior
{
    public override void ShipAction(TileBehavior tile)
    {
        //print("Destroyer");
        
        tile.AttackTile();
    }
}
