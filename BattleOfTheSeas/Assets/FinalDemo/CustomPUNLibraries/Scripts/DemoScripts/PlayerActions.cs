using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!TurnBasedSystem.Instance.IsPLayerTurnOver && 
                TurnBasedSystem.Instance.State == TurnBasedSystem.GameState.IN_PROGRESS)
            {
                print("EndingPlayerTurn");
                TurnBasedSystem.Instance.EndPlayerTurn();
            }
            else
            {
                print("Game is still in prep stage or its not your turn!");
            }
        }
    }
}
