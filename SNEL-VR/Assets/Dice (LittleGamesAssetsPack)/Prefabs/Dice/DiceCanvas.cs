using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCanvas : MonoBehaviour
{
    public GameObject diceMenu;
    private Canvas canvas;

    private void OnEnable()
    {
        canvas = diceMenu.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}
