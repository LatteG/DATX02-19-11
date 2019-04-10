using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMasterCreateNewPlayer : MonoBehaviour
{
    //Bug: Right now the rayTransform in UIHelpers/EventSystem is not set when spawning. Might be fixed by having only one UIHelper in the scene, and updating it when switching/spawning players.
    //Or just test it in VR

    private GameMasterManager gmm;

    //tmp
    private GameMasterChangeActivePlayer gmcap;

    public GameObject player;

    public void Start()
    {
        gmm = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterManager>();
        gmcap = GameObject.FindWithTag("GameMaster").GetComponent<GameMasterChangeActivePlayer>();

        //tmp
        SpawnPlayer();
    }

    //spawns one player at an empty spawn. (spawns at all of them atm)
    public void SpawnPlayer()
    {
        foreach(MeshCollider mc in gmm.GetAvailableSpawns())
        {
            if (!gmm.IsBusy(mc.GetHashCode()))
            {
                Vector3 pos = new Vector3(mc.bounds.min.x, mc.bounds.min.y, mc.bounds.min.z);
                GameObject newPlayer = Instantiate(player, pos, Quaternion.identity, transform.root.transform);
                gmm.UpdateSpawnBusy(mc.GetHashCode(), true);
                gmm.UpdateActivePlayers(newPlayer);
                Debug.Log("I'm alive!!");

                //tmp
                //gmcap.ChangePlayer(newPlayer);
                //break;
            }
        }
    }
}
