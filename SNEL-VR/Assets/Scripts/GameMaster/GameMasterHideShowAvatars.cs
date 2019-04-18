using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterHideShowAvatars : MonoBehaviour
{
    private GameMasterManager gmm;
    private GameMasterChangeActivePlayer gmcap;

    public void Start()
    {
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
        gmcap = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterChangeActivePlayer>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0)) ShowAvatars();
        if (Input.GetMouseButtonDown(1)) HideAvatars();
    }

    public void HideAvatars()
    {
        foreach (Player player in gmm.GetActivePlayers())
        {
            player.HideAvatar();
        }
    }

    public void ShowAvatars()
    {
        foreach (Player player in gmm.GetActivePlayers())
        {
            player.ShowAvatar();
        }

        gmcap.GetActivePlayer().HideAvatar();
        gmm.GetActivePlayers()[0].HideAvatar();
    }

}
