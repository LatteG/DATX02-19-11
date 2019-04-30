using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterBuildManager : MonoBehaviour
{
    private Dictionary<float, bool> spawnBusy;
    private List<MeshCollider> availableSpawns;
    private Transform itemPlatform;
    private int numSpawns;

    public void Start()
    {
        itemPlatform = GameObject.FindGameObjectWithTag("ItemPlatform").transform;
        numSpawns = 9;
        InitSpawns();    
    }

    //Search for MeshColliders and add them to hash map
    private void InitSpawns()
    {
        spawnBusy = new Dictionary<float, bool>();
        availableSpawns = new List<MeshCollider>();
        int count = 1;
       

        foreach (Transform child in itemPlatform)
        {
            
            if (numSpawns >= count)
            {
                spawnBusy.Add(child.GetComponent<MeshCollider>().GetHashCode(), false);
                availableSpawns.Add(child.GetComponent<MeshCollider>());

            }
            else child.gameObject.SetActive(false);

            count++;
            
        }

        Debug.Log("Spawn busy: " + spawnBusy.Count);
        Debug.Log("Avaiable spawns: " + availableSpawns.Count);

    }

    public void ResetAll()
    {
        spawnBusy.Clear();
        InitSpawns();

        foreach (MeshCollider mc in availableSpawns)
        {
            mc.gameObject.GetComponent<GameMasterBuildOnTriggerExit>().DestroyAll();
        }
    }

    //Checks if spot is free or not
    public bool IsBusy(float mc)
    {
        return spawnBusy[mc];
    }

    public void UpdateSpawnBusy(float mc, bool status)
    {
        spawnBusy[mc] = status;
    }

    public List<MeshCollider> GetAvailableSpawns()
    {
        return this.availableSpawns;
    }

    public void ClearSpawnBusy()
    {
        spawnBusy.Clear();
    }
}
