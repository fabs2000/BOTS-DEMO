using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipActionManager : MonoBehaviour
{
    [SerializeField] private ShipBehavior _parentShip;

    private Button _button;
    private int currentCD;
    
    void Start()
    {
        _button = GetComponent<Button>();
        currentCD = _parentShip.ActionCooldown;

        TurnBasedSystem.Instance.OnEndTurnCallbacks.AddListener(() => CheckForCd());
    }

    private void CheckForCd()
    {
        if (_button.enabled)
            return;
        
        currentCD--;

        if (currentCD == 0)
        {
            _button.enabled = true;
            currentCD = _parentShip.ActionCooldown;
        }
    }
}