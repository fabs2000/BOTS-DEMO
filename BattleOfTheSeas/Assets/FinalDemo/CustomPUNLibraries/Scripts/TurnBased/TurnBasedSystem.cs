using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TurnBasedSystem : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum GameState
    {
        PREPARATION,
        IN_PROGRESS,
        LOCAL_WON,
        OTHER_WON
    }

    #region Public Variables

    static public TurnBasedSystem Instance;

    public bool HasPreparationStage = true;

    [NonSerialized] public bool IsPLayerTurnOver;
    [NonSerialized] public int PlayerTurnID;
    [NonSerialized] public GameState State = GameState.IN_PROGRESS;

    [NonSerialized] public int TurnNumber = 1;

    #endregion

    #region Private Variables

    [Space(20)] [Tooltip("Duration Variables for Prep stage and turns measured in seconds")] [SerializeField]
    private float _prepStateDuration = 80f;

    [SerializeField] private float _turnDuration = 30f;

    private float _remainingPrepDuration, _remainingTurnDuration;
    private int _localPlayerID;

    private bool _gameEnded = false;

    #endregion

    #region Custom Event Callbacks
    
    [NonSerialized] public readonly UnityEvent OnBeginGameCallbacks = new UnityEvent();
    [NonSerialized] public readonly UnityEvent OnBeginTurnCallbacks = new UnityEvent();
    [NonSerialized] public readonly UnityEvent OnEndTurnCallbacks = new UnityEvent();

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
        print(PhotonNetwork.LocalPlayer.ActorNumber);
        _localPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;

        _remainingTurnDuration = _turnDuration;
        _remainingPrepDuration = _prepStateDuration;
    }

    #endregion

    #region PublicFunctions
    public void BeginGame()
    {
        //Begin game Callbacks
        OnBeginGameCallbacks.Invoke();

        //Game always starts with host player
        PlayerTurnID = 1;

        //Function begins game
        if (PlayerTurnID == _localPlayerID)
            StartCoroutine(BeginTurn());
    }
    public void BeginPreparationStage()
    {
        StartCoroutine(BeginPreparation());
    }
    public void EndPlayerTurn()
    {
        StopAllCoroutines();

        _remainingTurnDuration = _turnDuration;
        IsPLayerTurnOver = true;

        //Turn End Callbacks
        OnEndTurnCallbacks.Invoke();

        //Relays to all clients that it is now the next player's turn
        photonView.RPC("NextPlayerTurn", RpcTarget.All);
    }
    public float GetRemainingTime()
    {
        if (State == GameState.IN_PROGRESS)
        {
            return _remainingTurnDuration;
        }
        else if (State == GameState.PREPARATION)
        {
            return _remainingPrepDuration;
        }

        return 0f;
    }
    public void EndGame()
    {
        _gameEnded = true;
        StopAllCoroutines();
        //photonView.RPC("StopGame", RpcTarget.All);
    }

    #endregion

    #region PrivateFunctions

    [PunRPC]
    private void NextPlayerTurn()
    {
        if (!_gameEnded)
        {
            if (PlayerTurnID < PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                //Set the turn for the next player in line
                PlayerTurnID++;
            }
            else
            {
                //If the turn is at the last player, set back to host
                PlayerTurnID = 1;
            }

            TurnNumber++;

            if (PlayerTurnID == _localPlayerID)
                StartCoroutine(BeginTurn());
        }
        else
        {
            print("Game Ended");
        }
    }

    #endregion

    #region Coroutines
    private IEnumerator BeginTurn()
    {
        OnBeginTurnCallbacks.Invoke();

        State = GameState.IN_PROGRESS;

        IsPLayerTurnOver = false;
        while (!IsPLayerTurnOver)
        {
            _remainingTurnDuration -= Time.deltaTime;

            if (_remainingTurnDuration <= 0)
                EndPlayerTurn();

            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return null;
    }
    private IEnumerator BeginPreparation()
    {
        State = GameState.PREPARATION;

        _remainingPrepDuration = _prepStateDuration;

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
            stream.SendNext(HasPreparationStage);
        }
        else
        {
            _remainingPrepDuration = (float) stream.ReceiveNext();
            HasPreparationStage = (bool) stream.ReceiveNext();
        }
    }

    #endregion
}