using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class CheckFullRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]private UnityEvent BeginGameActions;
    
    void Start()
    {
        StartCoroutine(CheckPlayersEntered());
    }
    private IEnumerator CheckPlayersEntered()
    {
        TurnBasedSystem turnBasedSystem = TurnBasedSystem.Instance;
        
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                if(turnBasedSystem.HasPreparationStage)
                    turnBasedSystem.BeginPreparationStage();
                else
                    turnBasedSystem.BeginGame();

                // This calls whatever actions the game needs to go through at the beginning
                BeginGameActions.Invoke();

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
