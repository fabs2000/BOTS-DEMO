using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShipHealth : MonoBehaviour
{
    public int _shipHealth = 0;

    private PlayerManager _manager;
    private GameObject _shipMesh;
    private ShipBehavior _shipBehavior;

    private void Start()
    {
        _manager = FindObjectOfType<PlayerManager>();
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
}
