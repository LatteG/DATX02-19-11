using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterInit : MonoBehaviour
{
    GameMasterManager gmm;
    GameMasterCreateNewPlayer gmcnp;
    GameMasterChangeActivePlayer gmcap;
    GameMasterSwitchGMMode gmsgmm;

    void OnEnable()
    {
        gmm = gameObject.GetComponent<GameMasterManager>();
        gmcnp = gameObject.GetComponentInChildren<GameMasterCreateNewPlayer>();
        gmcap = gameObject.GetComponent<GameMasterChangeActivePlayer>();
        gmsgmm = gameObject.GetComponent<GameMasterSwitchGMMode>();

        gmm.enabled = true;
        gmcnp.enabled = true;
        gmcap.enabled = true;
        gmsgmm.enabled = true;
    }

}
