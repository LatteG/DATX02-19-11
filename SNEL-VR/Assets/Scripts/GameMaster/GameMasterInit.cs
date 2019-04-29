using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterInit : MonoBehaviour
{
    GameMasterManager gmm;
    GameMasterCreateNewPlayer gmcnp;
    GameMasterChangeActivePlayer gmcap;
    GameMasterSwitchGMMode gmsgmm;
    Figurine_SpawnFigurines fsf;

    void OnEnable()
    {
        gmm = gameObject.GetComponent<GameMasterManager>();
        gmcnp = gameObject.GetComponentInChildren<GameMasterCreateNewPlayer>();
        gmcap = gameObject.GetComponent<GameMasterChangeActivePlayer>();
        gmsgmm = gameObject.GetComponent<GameMasterSwitchGMMode>();
        fsf = GameObject.FindWithTag("FigurineSpawner").GetComponent<Figurine_SpawnFigurines>();

        gmm.enabled = true;
        gmcnp.enabled = true;
        gmcap.enabled = true;
        gmsgmm.enabled = true;
        fsf.enabled = true;
    }

}
