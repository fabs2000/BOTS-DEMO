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
        PROTECTIVE_DOME = 3,
        REPAIR = 4,
        BASIC_FIRE = 5,
        NO_ACTION = 6
    }
    
    #region Public
    
    public static PlayerManager Instance;

    public ActionType Action => _actionType;
    public ShipBehavior SelectedShip
    {
        get => _selectedShip;
        set => _selectedShip = value;
    }
    public GridBehaviour SelfGrid => _selfGrid;

    #endregion
    
    #region HUDVariables

    [SerializeField] private Button _rotLeft;
    [SerializeField] private Button _rotRight;

    private Text _endText;

    #endregion

    #region Private

    private ActionType _actionType = ActionType.NO_ACTION;
    private TurnBasedSystem _turnBasedSystem;

    private GameObject _playerBoard, _enemyBoard;
    private GridBehaviour _selfGrid, _enemyGrid;

    private ShipBehavior _selectedShip;

    #endregion
    
    #region MonoBehaviourCallbacks
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
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

    #region Public Functions
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

    public void SetActionType(int newActionType)
    {
        photonView.RPC("UpdateActionSelection", RpcTarget.All, newActionType);
    }

    public void ManageTileActions(TileBehaviour tile)
    {
        if (_actionType == ActionType.BASIC_FIRE)
        {
            //If the attack is the basic fire attack then the rest of the function can be skipped
            print("BASIC");
            tile.BasicAttack();
            return;
        }
        
        ShipBehavior attackingShip = DetermineAction();
        
        //Depending on the derived class that the ship is, it will perform the specific action
        attackingShip.ShipAction(tile);
        
        //Resets action type so it can't be used on cooldown
        _actionType = ActionType.NO_ACTION;
    }
    
    #endregion

    #region Private Functions
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

    private ShipBehavior DetermineAction()
    {
        foreach (var ship in _selfGrid.Ships)
        {
            if ((int) _actionType == (int) ship.ShipClass)
            {
                return ship;
            }
        }

        return null;
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

        //Enables raycast for tiles
        _enemyGrid.SetTilesLayer(0);

        //Disables raycast for player ships and tiles
        _selfGrid.SetTilesLayer(2);
        _selfGrid.SetShipLayer(2);

        //Set clone ship transforms
        _selfGrid.ReplicateShipTransforms();

        //Disables ship
        _selectedShip = null;
    }

    #endregion

    #region RPC

    [PunRPC]
    private void UpdateActionSelection(int newActionType)
    {
        _actionType = (ActionType) newActionType;
    }

    #endregion
    
}