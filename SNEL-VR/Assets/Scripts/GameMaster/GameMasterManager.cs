using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterManager : MonoBehaviour
{
    public GameObject gameMasterPlayer;
    //Hash map for spawns. Might be able to use only list instead, and add/remove from it
    private Dictionary<float, bool> spawnBusy;

    private List<MeshCollider> availableSpawns;

    private Transform playerSpawner;

    private List<GameObject> activePlayers;

    public void Start()
    {

        playerSpawner = transform.GetChild(0);

        activePlayers = new List<GameObject>();
        GameObject startPlayer = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Start player: "+startPlayer.GetHashCode());
        activePlayers.Add(startPlayer);

        InitSpawns();
    }

    //Search for MeshColliders and add them to hash map
    private void InitSpawns()
    {
        spawnBusy = new Dictionary<float, bool>();
        availableSpawns = new List<MeshCollider>();

        foreach (Transform child in playerSpawner)
        {
            spawnBusy.Add(child.GetComponent<MeshCollider>().GetHashCode(), false);
            availableSpawns.Add(child.GetComponent<MeshCollider>());

        }

        //Tmp
        Debug.Log(spawnBusy.Count);
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

    public void UpdateActivePlayers(GameObject newPlayer)
    {
        activePlayers.Add(newPlayer);
    }

    public List<GameObject> GetActivePlayers()
    {
        return this.activePlayers;
    }
}
