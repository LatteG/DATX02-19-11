using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayers : MonoBehaviour
{
    private GameMasterChangeActivePlayer tmp;

    public void Start()
    {
        tmp = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterChangeActivePlayer>();
    }

    public void KillOnClick()
    {
        //tmp.DeactivatePlayers();
    }
}
