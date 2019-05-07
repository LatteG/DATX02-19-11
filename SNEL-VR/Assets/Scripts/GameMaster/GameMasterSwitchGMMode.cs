using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterSwitchGMMode : MonoBehaviour
{
    private GameMasterManager gmm;
    private GameMasterChangeActivePlayer gmcap;
    private bool isGMMode;

    public GameObject gmCanvas;
    public GameObject gmnBuildManager;
    public GameMasterBuildManager gmbm;

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
            if(gmm.GetActivePlayers().Count > 1) DeactivateGMMode();
        }
    }
    private void ActivateGMMode()
    {
        isGMMode = true;
        gmCanvas.SetActive(true);
        //gmCanvas.transform.GetChild(2).gameObject.SetActive(false); //change

        gmnBuildManager.SetActive(true);

        gmcap.ChangePlayerGM();
        

    }

    private void DeactivateGMMode()
    {
        isGMMode = false;
        gmCanvas.SetActive(false);
        gmnBuildManager.SetActive(false);
        gmbm.ResetAll();
        gmcap.ChangePlayer();
        
    }

   
}
