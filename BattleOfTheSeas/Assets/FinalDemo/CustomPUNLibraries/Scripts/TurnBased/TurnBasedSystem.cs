using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnBasedSystem : MonoBehaviourPunCallbacks, IPunObservable
{
    static public TurnBasedSystem Instance;
    
    public enum GameState
    {
        PREPARATION,
        IN_PROGRESS,
        LOCAL_WON,
        OTHER_WON
    }
    
    #region Public Variables
    
    public bool HasPreparationStage;
    
    [NonSerialized] public bool IsPLayerTurnOver;
    [NonSerialized] public int PlayerTurnID;
    [NonSerialized] public GameState State = GameState.IN_PROGRESS;

    #endregion

    #region Private Variables

    [Space(20)]
    [Tooltip("Duration Variables for Prep stage and turns measured in seconds")]
    [SerializeField] private float _prepStateDuration = 80f;
    [SerializeField] private float _turnDuration = 30f;
    
    private float _remainingPrepDuration, _remainingTurnDuration;
    private int _localPlayerID;

    #endregion

    #region MonoBehaviourCallbacks
    private void Start()
    {
        Instance = this;
        
        print(PhotonNetwork.LocalPlayer.ActorNumber);
        _localPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;

        _remainingPrepDuration = _prepStateDuration;
        _remainingTurnDuration = _turnDuration;
    }

    private void Update()
    {
        if (State == GameState.IN_PROGRESS && IsPLayerTurnOver)
        {
            
        }
    }

    #endregion
    
    #region PublicFunctions
    public void BeginGame()
    {
        State = GameState.IN_PROGRESS;

        if (HasPreparationStage)
        {
            StartCoroutine(BeginPreparation());
        }
        else
        {
            //Game always starts with host player
            PlayerTurnID = 1;
            
            Debug.Log("Player turn: " + PlayerTurnID + ", Local Id: " + _localPlayerID);
            
            //Function begins game
            if (PlayerTurnID == _localPlayerID)
                StartCoroutine(BeginTurn());
            else
            {
                
            }
        }
    }
    public void EndPlayerTurn()
    {
        StopAllCoroutines();
        
        _remainingTurnDuration = _turnDuration;
        IsPLayerTurnOver = true;
        
        //Relays this function call to all clients
        photonView.RPC("NextPlayerTurn", RpcTarget.All);
    }
    public float GetRemainingTime()
    {
        if (State == GameState.IN_PROGRESS)
        {
            return _remainingTurnDuration;
        }
        else if(State == GameState.PREPARATION)
        {
            return _remainingPrepDuration;
        }

        return 0f;
    }

    #endregion

    #region PrivateFunctions
    [PunRPC]
    private void NextPlayerTurn()
    {
        if (PlayerTurnID < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PlayerTurnID++;
            print("Moved to next player: " + PlayerTurnID);
        }
        else
        {
            //If the turn is at the last player, set back to host
            PlayerTurnID = 1;
        }

        if (PlayerTurnID == _localPlayerID)
            StartCoroutine(BeginTurn());
    }

    #endregion

    #region Coroutines
    private IEnumerator BeginTurn()
    {
        print("Player's " + _localPlayerID + " turn is starting");

        //yield return new WaitForSeconds(3f);

        IsPLayerTurnOver = false;
        while (!IsPLayerTurnOver)
        {
            _remainingTurnDuration -= Time.deltaTime;
            
            if (_remainingTurnDuration <= 0)
            {
                EndPlayerTurn();
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return null;
    }
    private IEnumerator BeginPreparation()
    {
        State = GameState.PREPARATION;
        
        //print("Preparation stage will begin shortly!");
        //yield return new WaitForSeconds(3f);

        print("Preparation has begun!");

        while (HasPreparationStage)
        {
            _remainingPrepDuration -= Time.deltaTime;
            
            if (_remainingPrepDuration <= 0)
            {
                HasPreparationStage = false;
                _remainingPrepDuration = 0f;

                BeginGame();
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return null;
    }
    
    #endregion

    #region ValueSync
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_remainingPrepDuration);
            //stream.SendNext(_remainingTurnDuration);
        }
        else
        {
            _remainingPrepDuration = (float)stream.ReceiveNext();
            //_remainingTurnDuration = (float)stream.ReceiveNext();
        }
    }

    #endregion
}

