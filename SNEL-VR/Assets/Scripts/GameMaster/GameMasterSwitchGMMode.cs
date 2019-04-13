using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterSwitchGMMode : MonoBehaviour
{
    private GameMasterManager gmm;
    private GameMasterChangeActivePlayer gmcap;
    private bool isGMMode;
    private Player activePlayer; //remove

    public GameObject gmCanvas;

    public void OnEnable()
    {
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
        gmcap = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterChangeActivePlayer>();
        ActivateGMMode();
    }
    public void Update()
    {
        if ((Input.GetButtonDown("Fire3") || Input.GetButtonDown("Oculus_GearVR_LThumbstickX")) && !isGMMode)
        {
            ActivateGMMode();
        }
        else if((Input.GetButtonDown("Fire3") || Input.GetButtonDown("Oculus_GearVR_LThumbstickX")) && isGMMode)
        {
            DeactivateGMMode();
        }
    }
    private void ActivateGMMode()
    {
        gmCanvas.SetActive(true);
        gmCanvas.transform.GetChild(2).gameObject.SetActive(false); //change
        gmcap.ChangePlayerGM();
        isGMMode = true;

    }

    private void DeactivateGMMode()
    {
        isGMMode = false;
        gmCanvas.SetActive(false);
        gmcap.ChangePlayer();
        
    }

   
}
