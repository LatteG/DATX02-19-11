using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterSpawnBuildItem : MonoBehaviour
{
    private List<GameObject> spawnedObjects;
    private int count;

    public GameMasterBuildManager gmbm;
    public Transform figurineTransform;

    void Start()
    {
        spawnedObjects = new List<GameObject>(9);
    }
    public void SpawnItem(GameObject go)
    {
        //int count = 0;
        foreach (MeshCollider mc in gmbm.GetAvailableSpawns())
        {
            Debug.Log("Count: " + count);
            if (!gmbm.IsBusy(mc.GetHashCode())) {
                count++;
                Vector3 pos = mc.transform.position;
                //Transform spawnTransform = transform.GetChild(count);
                GameObject item = Instantiate(go, pos, Quaternion.identity, figurineTransform) as GameObject;
                //spawnedObjects[count] = item;
                gmbm.UpdateSpawnBusy(mc.GetHashCode(), true);
                break;
            }

            // Just so you can spawn more items if platform's full.
            // Later make it so that spawns which does not have anything on it becomes free.
            
            if (count == gmbm.GetAvailableSpawns().Count)
            { 
                gmbm.ResetAll();
                count = 0;

            }
            
        }

    }

}
