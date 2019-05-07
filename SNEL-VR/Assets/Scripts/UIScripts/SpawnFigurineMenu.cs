using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFigurineMenu : MonoBehaviour
{
    public GameObject figurineMenu;
    private bool menuEnabled = false;

    private void Start()
    {
        figurineMenu = gameObject.transform.GetChild(2).gameObject;
        figurineMenu.SetActive(false);
    }

    public void spawnMenu()
    {
        if(!menuEnabled)
        {
            figurineMenu.SetActive(true);
            menuEnabled = true;
        }
        else
        {
            figurineMenu.SetActive(false);
            menuEnabled = false;
        }

        
    }
}
