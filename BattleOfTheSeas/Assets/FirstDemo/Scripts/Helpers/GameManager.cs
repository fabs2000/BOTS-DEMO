using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance = null;
    public GameObject[] Boards;
    public enum GameState
    {
        PREPARATION,
        IN_PROGRESS,
        PLAYER_WON,
        ENEMY_WON
    }
    
    private GameState _gameState = GameState.PREPARATION;
    public GameState State
    {
        get => _gameState;
        set => _gameState = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // private IEnumerator EnemyFire()
    // {
    //     int randRange = Random.Range(0, 100);
    //     
    //     _enemyGrid.SetTilesLayer(2);
    //     
    //     yield return new WaitForSeconds(3);
    //         
    //     _playerGrid.Tiles[randRange].FireOnShip();
    //
    //     _thisPlayerTurn = true;
    //     
    //     _enemyGrid.SetTilesLayer(0);
    // }
}
