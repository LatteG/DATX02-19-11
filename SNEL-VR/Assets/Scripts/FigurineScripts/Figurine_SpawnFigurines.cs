﻿using System.Collections;
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
        //InitMaterials();
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
        availableSpawns = new List<MeshCollider>();
        InitSpawns();
        SpawnFigurines();

    }

    private void InitMaterials()
    {
        //figMaterials = new List<Material>();
        /*
        figMaterials.Add(Resources.Load("Materials/Blue") as Material);
        figMaterials.Add(Resources.Load("Materials/Black") as Material);
        figMaterials.Add(Resources.Load("Materials/Yellow") as Material);
        figMaterials.Add(Resources.Load("Materials/Red") as Material);
        figMaterials.Add(Resources.Load("Materials/Green") as Material);
        figMaterials.Add(Resources.Load("Materials/White") as Material);
        figMaterials.Add(Resources.Load("Materials/Pink") as Material);
        figMaterials.Add(Resources.Load("Materials/Orange") as Material);
        */
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
            pos.y += 0.5f;

            figurine.GetComponentInChildren<RotateCanvas>().target = targetTransform;
            figurine.GetComponentInChildren<Canvas>().worldCamera = eventCamera;
            figurine.GetComponentInChildren<OVRRaycaster>().pointer = pointer;

            GameObject figurine_tmp = Instantiate(figurine, pos, Quaternion.identity, transformFigurines) as GameObject;
            
            figurine_tmp.transform.GetChild(0).GetComponent<Renderer>().material = gmm.GetActivePlayers()[i].playerColor;

            gmm.GetActivePlayers()[i].AddFigurine(figurine_tmp);
            gmm.GetActivePlayers()[0].AddFigurine(figurine_tmp);
        }
    }


}
