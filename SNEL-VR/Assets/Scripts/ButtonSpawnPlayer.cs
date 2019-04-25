using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSpawnPlayer : MonoBehaviour
{
    private GameMasterCreateNewPlayer gmcnp;
    private GameMasterChangeActivePlayer tmp;

    public void Start()
    {
        gmcnp = GameObject.FindWithTag("PlayerSpawner").GetComponent<GameMasterCreateNewPlayer>();
        //tmp
        //tmp = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterChangeActivePlayer>();
    }

    public void SpawnOnClick()
    {
        gmcnp.SpawnPlayer();

        //tmp
       // tmp.DeactivatePlayers();
    }
}
