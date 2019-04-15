using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figurine_SpawnFigurines : MonoBehaviour
{
    private GameMasterManager gmm;  
    private List<MeshCollider> availableSpawns;

    public GameObject figurine;
    public Transform transformFigurines;

    public void OnEnable()
    {
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
        availableSpawns = new List<MeshCollider>();
        InitSpawns();
        SpawnFigurines();

    }

    private void InitSpawns()
    {
        foreach (Transform spawnPoint in this.transform)
        {
            availableSpawns.Add(spawnPoint.GetComponent<MeshCollider>());
        }

        Debug.Log("Num fig spawns: " + availableSpawns.Count);
    }


    private void SpawnFigurines()
    {
        for(int i = 1; i < gmm.GetActivePlayers().Count; i++)
        {
            Vector3 pos = availableSpawns[i-1].transform.position;
            GameObject figurine_tmp = Instantiate(figurine, pos, Quaternion.identity, transformFigurines);
            gmm.GetActivePlayers()[i].AddFigurine(figurine_tmp);
            gmm.GetActivePlayers()[0].AddFigurine(figurine_tmp);
        }
    }
}
