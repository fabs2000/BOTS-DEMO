using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cruiser : ShipBehavior
{
    public override void ShipAction(TileBehaviour tile)
    {
        //print("Cruiser");

        tile.RestoreTile();
    }
}
