using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviourPun
{
    public enum ActionType
    {
        AIR_ATTACKS = 0,
        TORPEDO = 1,
        PIERCE_MISSILE = 2,
        BASIC_FIRE = 3,
        PROTECTIVE_DOME = 4,
        REPAIR = 5
    }
    
    #region HUDVariables

    [SerializeField] private Button _rotLeft;
    [SerializeField] private Button _rotRight;

    private Text _endText;

    #endregion

    #region Private

    private ActionType _actionType = ActionType.BASIC_FIRE;
    private TurnBasedSystem _turnBasedSystem;

    private GameObject _playerBoard, _enemyBoard;
    private GridBehaviour _selfGrid, _enemyGrid;

    private ShipBehavior _selectedShip;

    #endregion

    #region Public

    public ActionType Action => _actionType;
    public ShipBehavior SelectedShip => _selectedShip;

    #endregion

    #region MonoBehaviourCallbacks

    private void Start()
    {
        _turnBasedSystem = TurnBasedSystem.Instance;
        
        if (_rotLeft && _rotRight)
        {
            Menu endGame = MenuManager.Instance.GetMenu("EndGame");
            
            _rotLeft.onClick.AddListener(() => RotateShip(-1));
            _rotRight.onClick.AddListener(() => RotateShip(1));
            
            _turnBasedSystem.OnPrepEndedCallbacks.AddListener(() => LockShips());
            
            _endText = endGame.GetComponentInChildren<Text>();
        }
        else
        {
            Debug.LogError("HUD Buttons not loaded");
        }

        StartCoroutine(GameStateActions());
    }

    #endregion

    #region Custom

    //Public
    public void SetBoards(GameObject board1, GameObject board2)
    {
        _playerBoard = board1;
        _enemyBoard = board2;

        if (_playerBoard && _enemyBoard)
        {
            _selfGrid = _playerBoard.GetComponentInChildren<GridBehaviour>();
            _enemyGrid = _enemyBoard.GetComponentInChildren<GridBehaviour>();
        }
    }

    public void SetActiveShip(ShipBehavior ship)
    {
        _selectedShip = ship;
    }

    public void RemoveShipFromGrid(GameObject ship)
    {
        if (_enemyGrid.FindShipToRemove(ship))
        {
            //Debug.Log("enemy ships now: " + _enemyGrid.Ships.Count);
            if (_enemyGrid.Ships.Count <= 0)
            {
                TurnBasedSystem.Instance.State = TurnBasedSystem.GameState.LOCAL_WON;
            }
        }
        else if (_selfGrid.FindShipToRemove(ship))
        {
            //Debug.Log("player ships now: " + _selfGrid.Ships.Count);
            if (_selfGrid.Ships.Count <= 0)
            {
                TurnBasedSystem.Instance.State = TurnBasedSystem.GameState.OTHER_WON;
            }
        }
    }

    public void SetActionType(int newActionType)
    {
        _actionType = (ActionType) newActionType;
    }

    public void ManageTileActions(int tileId)
    {
        switch (_actionType)
        {
            //Attacking Actions
            case ActionType.BASIC_FIRE:
                print("Basic");
                break;

            case ActionType.AIR_ATTACKS:
                print("Air");
                break;

            case ActionType.TORPEDO:
                print("Torpedo");
                break;

            case ActionType.PIERCE_MISSILE:
                print("Pierce");
                break;

            //Defensive Actions
            case ActionType.PROTECTIVE_DOME:
                print("Dome");
                break;

            case ActionType.REPAIR:
                print("Repair");
                break;
        }
    }

    //private functions
    private IEnumerator GameStateActions()
    {
        while (true)
        {
            if (!_turnBasedSystem) yield return null;

            switch (_turnBasedSystem.State)
            {
                case TurnBasedSystem.GameState.LOCAL_WON:
                    _endText.text = "YOU WON";
                    MenuManager.Instance.OpenMenu("EndGame");

                    StopAllCoroutines();
                    break;

                case TurnBasedSystem.GameState.OTHER_WON:
                    _endText.text = "ENEMY WON";
                    MenuManager.Instance.OpenMenu("EndGame");

                    StopAllCoroutines();
                    break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    #endregion

    #region UI

    private void RotateShip(int direction)
    {
        _selectedShip.transform.Rotate(Vector3.up, direction * 90);
    }

    private void LockShips()
    {
        MenuManager.Instance.OpenMenu("InGameHUD");

        //Disables raycast for enemy ships, enables for tiles
        _enemyGrid.SetShipLayer(2);
        _enemyGrid.SetTilesLayer(0);

        //Disables raycast for player ships and tiles
        _selfGrid.SetTilesLayer(2);
        _selfGrid.SetTilesLayer(2);

        //Set clone ship transforms
        _selfGrid.ReplicateShips();

        //Disables ship
        _selectedShip = null;
    }

    #endregion
}