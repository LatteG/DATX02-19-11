using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterChangeActivePlayer : MonoBehaviour
{
    //TODO: Make set positions more structurally decomposed
    private GameMasterManager gmm;
    private Player activePlayer;
    private Dictionary<int, Player> turnQueue;
    private int nextPlayer;
    private Transform tableTransform;



    // This needs to be called after the players have been spawned in
    public void OnEnable()
    {
        tableTransform = GameObject.FindWithTag("Table").transform;
        turnQueue = new Dictionary<int, Player>();
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
        SetActivePlayer(gmm.GetActivePlayers()[0]);
        InitTurnQueue();
        nextPlayer = 1;
    }

    public void ChangePlayer()
    {
        Debug.Log("In CP: Recent player was: " + activePlayer.name);
        Debug.Log("I had pos: " + activePlayer.GetPlayerPos());

        SetActivePlayer(turnQueue[nextPlayer]);
        SetPosition();

        Debug.Log("In CP: New player is: " + activePlayer.name);
        Debug.Log("I have pos: " + activePlayer.GetPlayerPos());

        nextPlayer = (nextPlayer + 1) % gmm.GetActivePlayers().Count;
        if (nextPlayer == 0) nextPlayer = 1;
    }

    public void ChangePlayerGM()
    {
        //Save position of previous player. 
        //SavePosition();
        //GM is placed at the same spot all the time.
        gmm.physicalPlayer.transform.position = gmm.GetGameMasterPos();

        Debug.Log("I'm the master!");
        Debug.Log("In CPGM: Recent player was: " + activePlayer.name);
    }

    public void SetActivePlayer(Player player)
    {
        this.activePlayer = player;
    }

    //Gives players from activePlayers a turn number
    public void InitTurnQueue()
    {
        List<int> usedNumbers = new List<int>();
        for (int i = 1; i < gmm.GetActivePlayers().Count; i++)
        {
            bool done = false;
            while (!done) { 
                int randomNumber = Random.Range(1, gmm.GetActivePlayers().Count);
                if (!usedNumbers.Contains(randomNumber))
                {
                    turnQueue.Add(randomNumber, gmm.GetActivePlayers()[i]);
                    usedNumbers.Add(randomNumber);
                    done = true;
                }
            }
        }

        Debug.Log("TurnQueue size: "+turnQueue.Count);

    }

    private void SavePosition()
    {
        int index = FindPlayer(activePlayer);
        gmm.GetActivePlayers()[index].SetPlayerPos(gmm.physicalPlayer.transform.position);

    }

    private void SetPosition()
    {
        int index = FindPlayer(activePlayer);
        gmm.physicalPlayer.transform.position = gmm.GetActivePlayers()[index].GetPlayerPos();

    }

  
    private int FindPlayer(Player player)
    {
        for(int i = 0; i < gmm.GetActivePlayers().Count; i++)
        {
            if (activePlayer.name.Equals(gmm.GetActivePlayers()[i].name))
            {
                return i;
            }
        }

        return -1;
    }
}
