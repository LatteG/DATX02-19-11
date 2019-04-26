using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMasterCreateNewPlayer : MonoBehaviour //change name to GameMasterCreateNewPlayers
{
    private GameMasterManager gmm;
    private int count;
    private Transform tableTransform;

    public GameObject player;

    public void OnEnable()
    {
        tableTransform = GameObject.FindWithTag("Table").transform;
        count = 1;
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
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
                pos.y  = 3.0f;

                Vector3 dir = tableTransform.position - pos;
                dir.y = 0; // keep the direction strictly horizontal
                Quaternion rot = Quaternion.LookRotation(dir);

                Player newPlayer = new Player();
                newPlayer.InitPlayer(pos, rot, count, "Player " + count);
                //Debug.Log("My name is: " + newPlayer.name);

                gmm.UpdateSpawnBusy(mc.GetHashCode(), true);
                gmm.UpdateActivePlayers(newPlayer);
                
                count++;
            }
        }

        //Debug.Log("Active players: " + gmm.GetActivePlayers().Count);
    }
}
