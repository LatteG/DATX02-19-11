using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figurine_SpawnFigurines : MonoBehaviour
{
    //Setting references for the canvas in player figurine when instantiating by finding them in the scene by sript doesn't work. 
    //They are set in the inspector instead. Then set before instantiation. 

    private GameMasterManager gmm;  
    private List<MeshCollider> availableSpawns;
    private Canvas figMenu;

    public Transform targetTransform;
    public Camera eventCamera;
    public GameObject pointer;
    
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

    }


    private void SpawnFigurines()
    {
        for(int i = 1; i < gmm.GetActivePlayers().Count; i++)
        {
            Vector3 pos = availableSpawns[i-1].transform.position;

            figurine.GetComponentInChildren<RotateCanvas>().target = targetTransform;
            figurine.GetComponentInChildren<Canvas>().worldCamera = eventCamera;
            figurine.GetComponentInChildren<OVRRaycaster>().pointer = pointer;

            GameObject figurine_tmp = Instantiate(figurine, pos, Quaternion.identity, transformFigurines) as GameObject;

            gmm.GetActivePlayers()[i].AddFigurine(figurine_tmp);
            gmm.GetActivePlayers()[0].AddFigurine(figurine_tmp);
        }
    }

}
