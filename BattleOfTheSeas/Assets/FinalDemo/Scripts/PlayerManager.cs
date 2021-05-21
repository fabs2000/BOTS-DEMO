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
    private TurnBasedSystem _turnBasedSystem;
    
    private GameObject _playerBoard;
    private GameObject _enemyBoard;

    [Header("HUD Variables")]
    [SerializeField] private Button _rotLeft;
    [SerializeField] private Button _rotRight;
    [SerializeField] private Button _lockShips;
    [SerializeField] private Image _endGame;

    private Text _endText;
    private GridBehaviour _selfGrid, _enemyGrid;
    
    private ShipBehavior _selectedShip;
    public ShipBehavior SelectedShip => _selectedShip;
    
    // private bool _thisPlayerTurn = false;
    // public bool ThisPlayerTurn => _thisPlayerTurn;

    #region MonoBehaviourCallbacks
    private void Start()
    {
        _turnBasedSystem = TurnBasedSystem.Instance;

        _turnBasedSystem.OnPrepEndedCallbacks.AddListener(() => LockShips());
        
        if (_rotLeft && _rotRight && _lockShips && _endGame)
        {
            _rotLeft.onClick.AddListener(() => RotateShip(-1));
            _rotRight.onClick.AddListener(() => RotateShip(1));
            _lockShips.onClick.AddListener(() => LockShips());
            _endText = _endGame.GetComponentInChildren<Text>();
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
            Debug.Log("enemy ships now: " + _enemyGrid.Ships.Count);
            if (_enemyGrid.Ships.Count <= 0)
            {
                TurnBasedSystem.Instance.State = TurnBasedSystem.GameState.LOCAL_WON;
            }
        }
        else if (_selfGrid.FindShipToRemove(ship))
        {
            Debug.Log("player ships now: " + _selfGrid.Ships.Count);
            if (_selfGrid.Ships.Count <= 0)
            {
                TurnBasedSystem.Instance.State = TurnBasedSystem.GameState.OTHER_WON;
            }
        }
    }

    // public void StartTurn()
    // {
    //     photonView.RPC("SetTurn", RpcTarget.Others, false);
    //     SetTurn(true);
    // }
    //
    // public void FinishTurn()
    // {
    //     photonView.RPC("SetTurn", RpcTarget.Others, true);
    //     SetTurn(false);
    // }
    //
    // //RPC
    // [PunRPC]
    // private void SetTurn(bool turn)
    // {
    //     _thisPlayerTurn = turn;
    // }

    //private functions
    private IEnumerator GameStateActions()
    {
        while(true)
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
        //If button has already been diasbled, don't call function again
        if (!_lockShips.IsInteractable())
            return;
            
        //Disables UI Buttons
        _rotLeft.interactable = false;
        _rotRight.interactable = false;
        _lockShips.interactable = false;

        //Disables raycast for enemy ships, enables for tiles
        _enemyGrid.SetShipLayer(2);
        _enemyGrid.SetTilesLayer(0);

        //Disables raycast for player ships and tiles
        _selfGrid.SetTilesLayer(2);
        _selfGrid.SetTilesLayer(2);
        
        //Set ship transforms
        _selfGrid.ReplicateShips();

        //Disables ship
        _selectedShip = null;
    }
    
    #endregion
}
