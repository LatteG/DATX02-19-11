using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterChangeActivePlayer : MonoBehaviour
{
    private GameMasterManager gmm;

    private Player activePlayer;

    public void Start()
    {
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
        SetActivePlayer(gmm.GetActivePlayers()[0]);
    }
    public void ChangePlayer(Player player)
    {
        activePlayer.SetPlayerPos(gmm.physicalPlayer.transform.position);
        SetActivePlayer(player);
        Vector3 pos = player.GetPlayerPos();
        pos.y += 4.0f;
        gmm.physicalPlayer.transform.position = pos;

    }


    public void SetActivePlayer(Player player)
    {
        this.activePlayer = player;
    }

  
}
