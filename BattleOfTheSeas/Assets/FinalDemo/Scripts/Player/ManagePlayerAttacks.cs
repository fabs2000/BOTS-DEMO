using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagePlayerAttacks : MonoBehaviour
{
    public enum ActionType
    {
        AIR_ATTACKS,
        AIR_DEFENSE,
        TORPEDO,
        REPAIR,
        PIERCE_MISSILE,
        BASIC_FIRE
    }
    private ActionType _actionType = ActionType.BASIC_FIRE;
    public ActionType actionType => _actionType;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #region Public Functions

    public void SetActionType(ActionType newActionType)
    {
        _actionType = newActionType;
    }

    #endregion
}
