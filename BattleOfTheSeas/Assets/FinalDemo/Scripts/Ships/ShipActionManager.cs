using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipActionManager : MonoBehaviour
{
    [SerializeField] private ShipBehavior _parentShip;

    private PlayerManager _player;
    
    private Button _button;
    private Text _text;
    private int currentCD;
    
    void Start()
    {
        currentCD = _parentShip.ActionCooldown;

        _button = GetComponent<Button>();
        _text = GetComponentInChildren<Text>();

        _player = PlayerManager.Instance;
        
        _button.onClick.AddListener(()=> 
            _player.SetActionType((int)_parentShip.ShipClass));
        
        TurnBasedSystem.Instance.OnEndTurnCallbacks.AddListener(CheckForCooldown);
    }

    public void StartCooldown()
    {
        _button.enabled = false;
        currentCD = _parentShip.ActionCooldown;
    }
 
    private void CheckForCooldown()
    {
        if ((int) _player.Action == (int) _parentShip.ShipClass)
            _button.enabled = false;
        
        if (_button.enabled)
            return;

        _text.text = currentCD.ToString();
        
        if (currentCD == 0)
        {
            _button.enabled = true;
            currentCD = _parentShip.ActionCooldown;
            _text.text = "";
        }
        
        --currentCD;
    }
}
