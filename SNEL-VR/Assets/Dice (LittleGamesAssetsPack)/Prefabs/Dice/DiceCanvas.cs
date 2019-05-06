using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCanvas : MonoBehaviour
{
    public GameObject diceMenu;
    //public GameObject cup;
    //public GameObject dice;
    //public Transform Spawn;


    private bool enabledMenu = false;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            enabledMenu = !enabledMenu;
            diceMenu.SetActive(enabledMenu);
        }
    }
}
