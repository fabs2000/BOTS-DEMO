using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShipHealth : MonoBehaviour
{
    public int _shipHealth = 0;

    private PlayerManager _manager;

    private void Start()
    {
        _manager = FindObjectOfType<PlayerManager>();
    }

    public void ShipHit()
    {
        --_shipHealth;

        if (_shipHealth <= 0)
        {
            _manager.RemoveShipFromGrid(gameObject);
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
