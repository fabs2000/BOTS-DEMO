using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Carrier : ShipBehavior
{
   public override void ShipAction(TileBehaviour tile)
   {
      print("Air Attack");

      TileBehaviour[] tiles = tile.ParentGrid.Tiles;
      int tileId = tile.TileID;
      
      //Center of the attack
      tile.BasicAttack();

      //Top
      tiles[tileId - 10].BasicAttack();
      //Bottom
      tiles[tileId + 10].BasicAttack();
      //Left
      tiles[tileId - 1].BasicAttack();
      //Right
      tiles[tileId + 1].BasicAttack();
   }
}
