using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : ShipBehavior
{
    public override void ShipAction(TileBehaviour tile)
    {
        print("Destroyer");
    }
}
