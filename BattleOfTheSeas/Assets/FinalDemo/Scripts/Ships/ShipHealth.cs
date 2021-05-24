using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShipHealth : MonoBehaviour
{
    public int _shipHealth = 0;
    public int _armor = 0;

    private PlayerManager _manager;
    private GameObject _shipMesh;
    private void Start()
    {
        _manager = FindObjectOfType<PlayerManager>();
        _shipMesh = transform.GetChild(0).gameObject;
    }

    public void ShipHit()
    {
        //Temp solution to armor, moving on the armor will be specific to areas of the ship.
        if (_armor > 0)
        {
            --_armor;
        }
        else
        {
            --_shipHealth;

            if (_shipHealth <= 0)
            {
                _manager.RemoveShipFromGrid(gameObject);
                
                _shipMesh.SetActive(true);
            }
        }
    }
}
