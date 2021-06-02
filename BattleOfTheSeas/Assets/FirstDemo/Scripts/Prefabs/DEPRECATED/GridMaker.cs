using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GridMaker : MonoBehaviour
{
    [Header("Template of tile to spawn")] public GameObject tileTemplate;
    [Space(20)] [SerializeField] private UnityEngine.Vector2Int gridDimensions;

    //[Range(0.5f, 3.0f)] [SerializeField] private float offsetBetweenTiles = 0.5f;

    [Space(20)] private GameObject[] _gridTiles;

    private Transform _backboard;
    

    //Creates the tiles to make a player's grid
    public void ConstructGrid()
    {
        _gridTiles = new GameObject[gridDimensions.x * gridDimensions.y];
        _backboard = transform.Find("Backboard");
        _backboard.localScale = new Vector3(gridDimensions.x + 1, 0.3f, gridDimensions.y + 1);
        
        Debug.Log("Size after construct: " + _gridTiles.Length);
        

        for (int i = 0; i < gridDimensions.x; i++)
        {
            for (int j = 0; j < gridDimensions.y; j++)
            {
                Vector3 pos = new Vector3(i - ((float) gridDimensions.x / 2 - (0.5f)), 0,
                    j - ((float) gridDimensions.y / 2 - (0.5f)));

                _gridTiles[i * j] = Instantiate(tileTemplate, pos, Quaternion.identity, _backboard);
            }
        }
    }

    //Clears tiles set up as a player's grid in the world
    public void ClearGrid()
    {
        

        Array.Clear(_gridTiles, 0, _gridTiles.Length);
        
        Debug.Log("Size of after clear: " + _gridTiles.Length);
    }
}