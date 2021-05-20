using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class CheckFullRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]private List<UnityEvent> BeginGameActions;
    
    void Start()
    {
        StartCoroutine(CheckPlayersEntered());
    }

    private IEnumerator CheckPlayersEntered()
    {
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                TurnBasedSystem.Instance.BeginGame();

                // This loops over whatever actions the game needs to go through
                // apart from calling begin on the turn system
                foreach (var action in BeginGameActions)
                {
                    action.Invoke();
                }
                
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
        Debug.Log("Player Joined");

        Debug.Log("Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}
