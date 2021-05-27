using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ShipHealth : MonoBehaviour
{
    public int _shipHealth = 0;
    public int _armor = 0;

    private PlayerManager _manager;
    private GameObject _shipMesh;
    private ShipBehavior _shipBehavior;

    private void Start()
    {
        _manager = PlayerManager.Instance;
        _shipMesh = transform.GetChild(0).gameObject;
        _shipBehavior = GetComponent<ShipBehavior>();
    }

    public void ShipHit()
    {
        --_shipHealth;
        
        if (_shipHealth <= 0)
        {
            _manager.SelfGrid.RemoveShipFromGrid(_shipBehavior);

            _shipMesh.SetActive(true);
        }
    }

    public void RestoreHealth()
    {
        ++_shipHealth;
    }
}
