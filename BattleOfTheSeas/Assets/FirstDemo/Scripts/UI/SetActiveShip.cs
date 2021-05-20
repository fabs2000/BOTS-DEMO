using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetActiveShip : MonoBehaviour
{
    private PlayerManager _manager;
    private Image _image;
    void Start()
    {
        _manager = FindObjectOfType<PlayerManager>();
        _image = GetComponent<Image>();

        //_manager.activeShipUI = _image;
    }
}
