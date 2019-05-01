using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public GameObject item;
    public GameMasterSpawnBuildItem gmsbi;

    //temp to test at home
    public void Update()
    {
        if (Input.GetButtonDown("Fire2")) Spawn_Item(); //left alt
    }

    public void Spawn_Item()
    {
        gmsbi.SpawnItem(item);
    }


}
