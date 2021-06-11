using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject TileSelector;
    
    public ActionType Action => _actionType;
    public ShipBehavior SelectedShip
    {
        get => _selectedShip;
        set => _selectedShip = value;
    }
    public GridBehaviour SelfGrid => _selfGrid;

    #endregion
    
    #region HUDVariables

    [Space(20)][Header("HUD Variables")]
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

        //At the beginning of each turn reset action type to none
        _turnBasedSystem.OnBeginTurnCallbacks.AddListener(()=>UpdateActionSelection(6));
        //_turnBasedSystem.OnEndTurnCallbacks.AddListener(GameStateActions);

        if (_rotLeft && _rotRight)
        {
            _rotLeft.onClick.AddListener(() => RotateShip(-1));
            _rotRight.onClick.AddListener(() => RotateShip(1));
            
            _turnBasedSystem.OnBeginGameCallbacks.AddListener(LockShips);

            Menu endGame = MenuManager.Instance.GetMenu("EndGame");
            _endText = endGame.GetComponentInChildren<Text>();
        }
        else
        {
            Debug.LogError("HUD Buttons not loaded");
        }
    }

    private void Update()
    {
        if (!_turnBasedSystem) return;
        
        switch (_turnBasedSystem.State)
        {
            case TurnBasedSystem.GameState.LOCAL_WON:
                _endText.text = "YOU WON";
                MenuManager.Instance.OpenMenu("EndGame");
                
                break;

            case TurnBasedSystem.GameState.OTHER_WON:
                _endText.text = "ENEMY WON";
                MenuManager.Instance.OpenMenu("EndGame");
                
                break;
        }
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
        if (newActionType == (int) ActionType.REPAIR || newActionType == (int) ActionType.PROTECTIVE_DOME)
        {
            //If it's a defensive action, these actions can only be done on the player's grid
            _enemyGrid.SetTilesLayer(2);
            _selfGrid.SetTilesLayer(0);
        }
        else
        {
            //If it's an attacking action, they can only be done on the enemy's grid.
            _enemyGrid.SetTilesLayer(0);
            _selfGrid.SetTilesLayer(2);
        }

        photonView.RPC("UpdateActionSelection", RpcTarget.All, newActionType);
    }

    public void ManageTileActions(TileBehavior tile)
    {
        if (_actionType == ActionType.BASIC_FIRE)
        {
            //If the attack is the basic fire attack then the rest of the function can be skipped
            tile.AttackTile();
            return;
        }
        
        //TODO: THE PROBLEM WITH "RPCs" NOT GETTING CALLED IS BECAUSE OF DUMB SHIT, WHEN ENEMY STILL HAS SHIP BUT YOU DONT, THE REPLICATION DOES NOT HAPPEN 
        
        ShipBehavior shipAction = DetermineAction();
        
        //Depending on the derived class that the ship is, it will perform the specific action
        if(shipAction)
            shipAction.ShipAction(tile);
    }
    
    public void RemoveShipFromGrid(ShipBehavior ship)
    {
        if (_enemyGrid.FindShipToRemove(ship))
        {
            Debug.Log("enemy ships now: " + _enemyGrid.ShipsRemaining);
            if (_enemyGrid.ShipsRemaining == 0)
            {
                print("Local Won");
                _turnBasedSystem.State = TurnBasedSystem.GameState.LOCAL_WON;
                _turnBasedSystem.EndGame();
            }
        }
        else if (_selfGrid.FindShipToRemove(ship))
        {
            Debug.Log("player ships now: " + _selfGrid.ShipsRemaining);
            if (_selfGrid.ShipsRemaining == 0)
            {
                print("Other Won");
                _turnBasedSystem.State = TurnBasedSystem.GameState.OTHER_WON;
                _turnBasedSystem.EndGame();
            }
        }
    }
    
    #endregion

    #region Private Functions
    
    //private void GameStateActions()
    // {
    //     if (!_turnBasedSystem) return;
    //     
    //     switch (_turnBasedSystem.State)
    //     {
    //         case TurnBasedSystem.GameState.LOCAL_WON:
    //             _endText.text = "YOU WON";
    //             MenuManager.Instance.OpenMenu("EndGame");
    //             
    //             break;
    //
    //         case TurnBasedSystem.GameState.OTHER_WON:
    //             _endText.text = "ENEMY WON";
    //             MenuManager.Instance.OpenMenu("EndGame");
    //             
    //             break;
    //     }
    // }

    private ShipBehavior DetermineAction()
    {
        foreach (var ship in _selfGrid.Ships)
        {
            if ((int)_actionType == (int)ship.ShipClass)
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
        _enemyGrid.SetShipLayer(2);

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