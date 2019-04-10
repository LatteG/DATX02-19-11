using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMasterCreateNewPlayer : MonoBehaviour
{
    private GameMasterManager gmm;
    private int count;

    //tmp
    private GameMasterChangeActivePlayer gmcap;

    public GameObject player;

    public void Start()
    {
        count = 1;
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
        gmcap = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterChangeActivePlayer>();

        //tmp
        SpawnPlayer();
    }

    //spawns one player at an empty spawn. (spawns at all of them atm)
    public void SpawnPlayer()
    {
        foreach(MeshCollider mc in gmm.GetAvailableSpawns())
        {
            if (!gmm.IsBusy(mc.GetHashCode()))
            {
                Vector3 pos = mc.transform.position;
                pos.y += 4.0f;
                Player newPlayer = new Player();
                newPlayer.InitPlayer(pos, count, "Player " + count);
                Debug.Log("My name is: " + newPlayer.name);

                gmm.UpdateSpawnBusy(mc.GetHashCode(), true);
                gmm.UpdateActivePlayers(newPlayer);
                Debug.Log("I'm alive!!");
                count++;
            }
        }
    }
}
