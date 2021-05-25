using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleship : ShipBehavior
{
    public override void ShipAction(TileBehaviour tile)
    {
        print("Battleship");
    }
}
