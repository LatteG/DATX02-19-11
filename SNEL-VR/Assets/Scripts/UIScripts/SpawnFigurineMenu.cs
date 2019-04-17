using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFigurineMenu : MonoBehaviour
{
    public Canvas figurineMenu;
    private bool menuEnabled = false;
    private Vector3 pos = new Vector3(15,2,4);

    private void Start()
    {
        figurineMenu.enabled = false;
    }

    public void spawnMenu()
    {
        if(!menuEnabled)
        {
            figurineMenu.enabled = true;
            //Camera MyCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            //GameObject Mypointer = GameObject.FindWithTag("LaserPointer").GetComponent<GameObject>();
            //OVRRaycaster laser = figurineMenu.GetComponent<OVRRaycaster>().GetComponentInChildren<OVRRaycaster>();
            //figurineMenu.worldCamera = MyCamera;
            //laser.pointer = Mypointer;
            menuEnabled = true;
        }
        else
        {
            figurineMenu.enabled = false;
            menuEnabled = false;
        }

        
    }
}
