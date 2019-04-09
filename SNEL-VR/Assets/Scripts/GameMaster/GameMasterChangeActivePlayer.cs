using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterChangeActivePlayer : MonoBehaviour
{
    private GameMasterManager gmm;

    private GameObject activePlayer;

    public void Start()
    {
        //SetActivePlayer();
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
    }
    public void ChangePlayer(GameObject player)
    {
        //SetActivePlayer();
        activePlayer = player;
        DeactivatePlayers();
        activePlayer.SetActive(true);
        Debug.Log("Active player: " + activePlayer.GetHashCode());
    }

    //Deactivates all other players (doesn't work atm)
    public void DeactivatePlayers()
    {
        
        List<GameObject> activePlayers = gmm.GetActivePlayers();

        foreach (GameObject player in activePlayers)
        {
            if (!GameObject.ReferenceEquals(player, activePlayer))
            {
                player.SetActive(false);
            }
        }

        
    }

    //Fuuulhack
    private GameObject FindTopParent(Camera c)
    {
        Transform t = c.transform;
        for (int i = 0; i<10; i++)
        {
            
            if (t.parent.CompareTag("Player"))
            {
                Debug.Log("YAAAY");
                return t.parent.gameObject;
                
            }
            t = t.parent.transform;
        }

        return null;
    }

    private void SetActivePlayer()
    {
        Debug.Log("Main camera: " + Camera.main);
        activePlayer = FindTopParent(Camera.main);
        Debug.Log("ActivePlayer: " + activePlayer);
    }
}
