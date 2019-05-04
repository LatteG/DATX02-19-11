using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMasterChangeActivePlayer : MonoBehaviour
{
    private GameMasterManager gmm;

    //change later to make dependencies better
    private PlayerOwnedFigurines pof;

    private Player activePlayer;
    private Dictionary<int, Player> turnQueue;
    private int nextPlayer;
    private Transform tableTransform;
    private bool isGM;
    private Camera mainCamera;

    public Text text;



    // This needs to be called after the players have been spawned in
    public void OnEnable()
    {
        tableTransform = GameObject.FindWithTag("Table").transform;
        turnQueue = new Dictionary<int, Player>();
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
        pof = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerOwnedFigurines>();
        SetActivePlayer(gmm.GetActivePlayers()[0]);
        InitTurnQueue();
        nextPlayer = 1;
        isGM = true;
        mainCamera = Camera.main;

    }

    public void ChangePlayer()
    {
        isGM = false;
       
        SetActivePlayer(turnQueue[nextPlayer]);
        SetPosition();

        UpdatePlayerAvatar();

        nextPlayer = (nextPlayer + 1) % gmm.GetActivePlayers().Count;

        //Since turnQueue doesn't have the key 0
        if (nextPlayer == 0) nextPlayer = 1;

        //change later to make dependencies better
        pof.ChangeOwnedFigurines(gmm.GetActivePlayers()[FindPlayer(activePlayer)].GetOwnedFigurines());

        mainCamera.cullingMask = ~(1<<15);

        text.text = "Current player: "+activePlayer.playerColor.name;


    }

    public void ChangePlayerGM()
    {
        isGM = true;
        //Save position of previous player. 
        SavePosition();

        //GM is placed at the same spot all the time.
        gmm.physicalPlayer.transform.GetChild(0).position = gmm.GetGameMasterPos();
        Debug.Log("GM y-pos: " + gmm.physicalPlayer.transform.GetChild(0).position.y);
        gmm.physicalPlayer.transform.GetChild(0).rotation = gmm.GetGameMasterRotation();
        UpdatePlayerAvatar();

        //change later to make dependencies better
        pof.ChangeOwnedFigurines(gmm.GetActivePlayers()[0].GetOwnedFigurines());

        mainCamera.cullingMask = ~(0);

        if (activePlayer.playerColor != null)
        {
            text.text = "GM is active. " +"\nPrevious player: "+ activePlayer.playerColor.name;
        }
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

        //Debug.Log("TurnQueue size: "+turnQueue.Count);

    }

    public Player GetActivePlayer()
    {
        int index = FindPlayer(activePlayer);
        return gmm.GetActivePlayers()[index];
    }

    public bool GetIsGM()
    {
        return this.isGM;
    }

    private void SavePosition()
    {
        int index = FindPlayer(activePlayer);
        gmm.GetActivePlayers()[index].SetPlayerPos(gmm.physicalPlayer.transform.GetChild(0).position);
        gmm.GetActivePlayers()[index].SetPlayerRotation(gmm.physicalPlayer.transform.GetChild(0).rotation);

    }

    private void SetPosition()
    {
        int index = FindPlayer(activePlayer);
        gmm.physicalPlayer.transform.GetChild(0).position = gmm.GetActivePlayers()[index].GetPlayerPos();
        gmm.physicalPlayer.transform.GetChild(0).rotation = gmm.GetActivePlayers()[index].GetPlayerRotation();

    }

    //Might not be needed, but to prevent pointer bugs.
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

    private void UpdatePlayerAvatar()
    {
        foreach(Player player in gmm.GetActivePlayers())
        {
            if (!player.name.Equals(activePlayer.name));
            {
                player.ShowAvatar();
            }
        }

        int index = FindPlayer(activePlayer);

        gmm.GetActivePlayers()[index].MoveAvatar();
        if (!isGM) gmm.GetActivePlayers()[index].HideAvatar();
        gmm.GetActivePlayers()[0].HideAvatar();

    }
}
