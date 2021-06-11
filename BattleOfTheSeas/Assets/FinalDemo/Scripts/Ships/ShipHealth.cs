using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ShipHealth : MonoBehaviourPun
{
    public int _shipHealth = 0;

    private GameObject _shipMesh;
    private ShipBehavior _ship;
    private PlayerManager _manager;
    
    private void Start()
    {
        _shipMesh = transform.GetChild(0).gameObject;
        _ship = GetComponent<ShipBehavior>();
        _manager = FindObjectOfType<PlayerManager>();
    }

    public void ShipHit()
    {
        --_shipHealth;

        if (_shipHealth <= 0)
        {
            _manager.RemoveShipFromGrid(_ship);
            _shipMesh.SetActive(true);
        }
    }

    public void RestoreHealth()
    {
        ++_shipHealth;
    }
}
