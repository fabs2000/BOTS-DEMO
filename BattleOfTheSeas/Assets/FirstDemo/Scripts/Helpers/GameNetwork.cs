using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetwork : MonoBehaviourPunCallbacks
{
    [Header("UI variables")]
    [SerializeField] private GameObject _HUD;
    [SerializeField] private GameObject _Waiting;

    
    [Space(20)] [SerializeField] private Transform[] _spawns;
    [SerializeField] private PlayerManager _player;
    [SerializeField] private Camera _initialCamera;

    private void Start()
    {
        // if (!_playerPrefab)
        // {
        //     Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
        // }

        StartCoroutine(CheckPlayersEntered());
    }
    
    private IEnumerator CheckPlayersEntered()
    {
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                //Begin Game, assign each player to their own cameras
                _HUD.SetActive(true);
                _Waiting.SetActive(false);

                // if (PhotonNetwork.IsMasterClient)
                // {
                //     //Debug.LogFormat("We are Instantiating HostPlayer at pos: " + _spawnPoints[0].position, SceneManagerHelper.ActiveSceneName);
                //
                //     Instantiate(_playerPrefab, _spawnPoints[0].position, _spawnPoints[0].rotation);
                // }
                // else
                // {
                //     //Debug.LogFormat("We are Instantiating GuestPlayer at pos: " + _spawnPoints[1].position, SceneManagerHelper.ActiveSceneName);
                //
                //     Instantiate(_playerPrefab, _spawnPoints[1].position, _spawnPoints[1].rotation);
                // }

                Transform playerTrasf = _player.transform;
                
                
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("We are Instantiating HostPlayer");
                    
                    playerTrasf.position = _spawns[0].position;
                    playerTrasf.rotation = _spawns[0].rotation;
                    playerTrasf.localScale = _spawns[0].localScale;
                    
                    _player.SetBoards(GameManager.Instance.Boards[0], GameManager.Instance.Boards[1]);
                }
                else
                {
                    Debug.Log("We are Instantiating GuestPlayer");
                    
                    playerTrasf.position = _spawns[1].position;
                    playerTrasf.rotation = _spawns[1].rotation;
                    playerTrasf.localScale = _spawns[1].localScale;
                    
                    _player.SetBoards(GameManager.Instance.Boards[2], GameManager.Instance.Boards[3]);
                }
                
                _initialCamera.enabled = false;

                StopAllCoroutines();
            }
            else
            {
                //Do Nothing
                print("Only " + PhotonNetwork.CurrentRoom.PlayerCount + " players out of "
                      + PhotonNetwork.CurrentRoom.MaxPlayers + " in room, waiting for more players");
            }
            
            yield return new WaitForSeconds(1f);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName); 
        
        print("Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}