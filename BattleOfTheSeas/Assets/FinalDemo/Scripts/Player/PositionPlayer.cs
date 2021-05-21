using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PositionPlayer : MonoBehaviourPun
{
    [Space(20)] [SerializeField] private Transform[] _spawns;
    [SerializeField] private PlayerManager _player;
    [SerializeField] private Camera _initialCamera;

    public void InitPlayer()
    {
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
    }
}
