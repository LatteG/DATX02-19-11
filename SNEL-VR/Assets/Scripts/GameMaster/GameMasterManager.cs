using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterManager : MonoBehaviour
{
    public GameObject physicalPlayer;
    public int playerAmount;

    private Dictionary<float, bool> spawnBusy;
    private List<MeshCollider> availableSpawns;
    private Transform playerSpawner;
    private List<Player> activePlayers;
    private Vector3 gameMasterPos;
    private Quaternion gameMasterRotation;

    public void OnEnable()
    {

        playerSpawner = transform.GetChild(0);

        activePlayers = new List<Player>();

        //physicalPlayer = GameObject.FindGameObjectWithTag("Player"); //change tag to GameMaster

        Player gameMaster = new Player();

        //float x = physicalPlayer.transform.GetChild(0).position.x;  
        //float y = 3.0f;
        //float z = physicalPlayer.transform.GetChild(0).position.z;
        //gameMasterPos = new Vector3(x, y, z);

        gameMasterPos = new Vector3(12, 3, -1.5f);
        gameMasterRotation = physicalPlayer.transform.GetChild(0).rotation;

        gameMaster.InitPlayer(gameMasterPos, gameMasterRotation, 0, "GameMaster");

        activePlayers.Add(gameMaster);

        InitSpawns();
        

    }

    //Search for MeshColliders and add them to hash map
    private void InitSpawns()
    {
        spawnBusy = new Dictionary<float, bool>();
        availableSpawns = new List<MeshCollider>();
        int count = 1;

        foreach (Transform child in playerSpawner)
        {
            if (child.gameObject.active) //this if-statement is maybe obsolete
            {
                if (playerAmount >= count)
                {
                    spawnBusy.Add(child.GetComponent<MeshCollider>().GetHashCode(), false);
                    availableSpawns.Add(child.GetComponent<MeshCollider>());

                }else child.gameObject.SetActive(false);

                count++;
            }
        }

        //Debug.Log("Spawn busy: " + spawnBusy.Count);
        //Debug.Log("Avaiable spawns: " + availableSpawns.Count);

    }

  

    //Checks if spot is free or not
    public bool IsBusy(float mc)
    {
        return spawnBusy[mc];
    }

    //In case we want to be able to add spawn points in game
    public void AddRemoveSpawnBusy(float mc, bool add)
    {
        if (add) spawnBusy.Add(mc, false);
        else spawnBusy.Remove(mc);
    }

    public void UpdateSpawnBusy(float mc, bool status)
    {
        spawnBusy[mc] =  status;
    }

    public Dictionary<float, bool> GetSpawnBusy()
    {
        return this.spawnBusy;
    }

    public List<MeshCollider> GetAvailableSpawns()
    {
        return this.availableSpawns;
    }

    public void UpdateActivePlayers(Player newPlayer)
    {
        activePlayers.Add(newPlayer);
    }

    public List<Player> GetActivePlayers()
    {
        return this.activePlayers;
    }

    public Vector3 GetGameMasterPos()
    {
        return this.gameMasterPos;
    }

    internal Quaternion GetGameMasterRotation()
    {
        return this.gameMasterRotation;
    }
}
