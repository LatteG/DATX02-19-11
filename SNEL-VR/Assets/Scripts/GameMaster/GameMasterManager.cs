using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterManager : MonoBehaviour
{
    public GameObject physicalPlayer;
    private Dictionary<float, bool> spawnBusy;
    private List<MeshCollider> availableSpawns;
    private Transform playerSpawner;
    private List<Player> activePlayers;

    public void OnEnable()
    {

        playerSpawner = transform.GetChild(0);

        activePlayers = new List<Player>();
        physicalPlayer = GameObject.FindGameObjectWithTag("Player"); //change tag to GameMaster

        Player gameMaster = new Player();
        float x = physicalPlayer.transform.position.x;  
        float y = physicalPlayer.transform.position.y;
        float z = physicalPlayer.transform.position.z;
        Vector3 gameMasterPos = new Vector3(x, y, z);

        gameMaster.InitPlayer(gameMasterPos, physicalPlayer.transform.rotation, 0, "GameMaster");

        activePlayers.Add(gameMaster);

        InitSpawns();

    }

    //Search for MeshColliders and add them to hash map
    private void InitSpawns()
    {
        spawnBusy = new Dictionary<float, bool>();
        availableSpawns = new List<MeshCollider>();

        foreach (Transform child in playerSpawner)
        {
            if (child.gameObject.active)
            {
                spawnBusy.Add(child.GetComponent<MeshCollider>().GetHashCode(), false);
                availableSpawns.Add(child.GetComponent<MeshCollider>());
            }
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

    public void UpdateActivePlayers(Player newPlayer)
    {
        activePlayers.Add(newPlayer);
    }

    public List<Player> GetActivePlayers()
    {
        return this.activePlayers;
    }
}
