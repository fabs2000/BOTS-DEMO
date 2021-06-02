using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cruiser : ShipBehavior
{
    public override void ShipAction(TileBehavior tile)
    {
        //print("Cruiser");

        tile.RestoreTile();
    }
}
