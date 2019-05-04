using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCanvas : MonoBehaviour
{
    public GameObject diceMenu;
    public GameObject cup;
    public GameObject dice;

    private Vector3 pos = new Vector3(16.5f,2,3.3f);

    private bool enabledMenu = false;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two) && !enabledMenu)
        {
            diceMenu.SetActive(true);
            enabledMenu = true;
            Instantiate(dice, pos, Quaternion.identity);
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two) && enabledMenu)
        {
            diceMenu.SetActive(false);
            enabledMenu = false;
        }
    }
}
