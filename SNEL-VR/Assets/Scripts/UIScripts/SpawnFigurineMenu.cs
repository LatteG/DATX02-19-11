using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFigurineMenu : MonoBehaviour
{
    public Canvas figurineMenu;
    private bool menuEnabled = false;

    

    public void spawnMenu()
    {
        if(!menuEnabled)
        {
            Instantiate(figurineMenu);
            Camera MyCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            GameObject Mypointer = GameObject.FindWithTag("LaserPointer").GetComponent<GameObject>();
            OVRRaycaster laser = figurineMenu.GetComponent<OVRRaycaster>().GetComponentInChildren<OVRRaycaster>();
            figurineMenu.worldCamera = MyCamera;
            laser.pointer = Mypointer;
            menuEnabled = true;
        }
        else
        {
            Destroy(figurineMenu);
            menuEnabled = false;
        }

        
    }
}
