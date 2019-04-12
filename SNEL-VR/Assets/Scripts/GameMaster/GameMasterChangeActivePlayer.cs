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
        Debug.Log("NextPlayer: " + nextPlayer);
        Debug.Log("PlayerName: " + turnQueue[nextPlayer].name);

        int index = FindPlayer(activePlayer);
        gmm.GetActivePlayers()[index].SetPlayerPos(gmm.physicalPlayer.transform.position);
        gmm.GetActivePlayers()[index].SetPlayerRotation(gmm.physicalPlayer.transform.rotation);
        
        SetActivePlayer(turnQueue[nextPlayer]);
        SetPositions();
       
        nextPlayer = (nextPlayer + 1) % gmm.GetActivePlayers().Count;
        if (nextPlayer == 0) nextPlayer += 1;

        
    }

    public void ChangePlayerGM()
    {
        Debug.Log("PlayerName: " + gmm.GetActivePlayers()[0].name);

        int index = FindPlayer(activePlayer);
        //gmm.GetActivePlayers()[index].SetPlayerPos(gmm.physicalPlayer.transform.position);
        //gmm.GetActivePlayers()[index].SetPlayerRotation(gmm.physicalPlayer.transform.rotation);

        activePlayer.SetPlayerPos(gmm.physicalPlayer.transform.position);
        activePlayer.SetPlayerRotation(gmm.physicalPlayer.transform.rotation);

        SetActivePlayer(gmm.GetActivePlayers()[0]);
        SetPositions();
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

    private void SetPositions()
    {
        int index = FindPlayer(activePlayer);
        //Vector3 pos = gmm.GetActivePlayers()[index].GetPlayerPos();
        Vector3 pos = activePlayer.GetPlayerPos();
        pos.y += 4.0f;
        

        Vector3 dir = tableTransform.position - pos;
        dir.y = 0; // keep the direction strictly horizontal
        //Quaternion rotation = Quaternion.LookRotation(dir);
        //Quaternion rotation = gmm.GetActivePlayers()[index].GetPlayerRotation();
        Quaternion rotation = activePlayer.GetPlayerRotation();
        //gmm.physicalPlayer.transform.position = pos;
        //gmm.physicalPlayer.transform.SetPositionAndRotation(pos, rotation);
        gmm.physicalPlayer.transform.position = pos;
        //Camera mainCamera = Camera.main;
        //mainCamera.transform.LookAt(tableTransform);
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
