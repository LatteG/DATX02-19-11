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
        //press 1-9 on the keyboard
        //yield return StartCoroutine(ChoosePlayer());

        isGMMode = false;
        gmCanvas.SetActive(false);
        //activePlayer = gmm.GetActivePlayers()[1];
        gmcap.ChangePlayer();
        
    }

    //Change later... Detects which number is pressed and takes the corresponding player. Max 9 players atm. 
    //Doesn't work
    private IEnumerator ChoosePlayer()
    {
        string keyPressed = Input.inputString;
        Debug.Log("keyPressed: " + keyPressed);
        switch (keyPressed)
        {
            case "Alpha1":
                activePlayer = gmm.GetActivePlayers()[1];
                break;
            case "Alpha2":
                activePlayer = gmm.GetActivePlayers()[2];
                break;
            case "Alpha3":
                activePlayer = gmm.GetActivePlayers()[3];
                break;
            case "Alpha4":
                activePlayer = gmm.GetActivePlayers()[4];
                break;
            case "Alpha5":
                activePlayer = gmm.GetActivePlayers()[5];
                break;
            case "Alpha6":
                activePlayer = gmm.GetActivePlayers()[6];
                break;
            case "Alpha7":
                activePlayer = gmm.GetActivePlayers()[7];
                break;
            case "Alpha8":
                activePlayer = gmm.GetActivePlayers()[8];
                break;
            case "Alpha9":
                activePlayer = gmm.GetActivePlayers()[9];
                break;
        }

        yield return null;
    }

   
}
