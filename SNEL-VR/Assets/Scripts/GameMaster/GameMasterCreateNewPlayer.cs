using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMasterCreateNewPlayer : MonoBehaviour
{
    private GameMasterManager gmm;
    public GameObject player;

    public void Start()
    {
        gmm = transform.parent.GetComponent<GameMasterManager>();
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        foreach(MeshCollider mc in gmm.GetAvailableSpawns())
        {
            if (!gmm.IsBusy(mc.GetHashCode()))
            {
                Vector3 pos = new Vector3(mc.bounds.max.x, mc.bounds.max.y, mc.bounds.max.z);
                GameObject newPlayer = Instantiate(player, pos, Quaternion.identity, mc.transform);
                gmm.UpdateSpawnBusy(mc.GetHashCode(), true);
                break;
            }
        }
    }
}
