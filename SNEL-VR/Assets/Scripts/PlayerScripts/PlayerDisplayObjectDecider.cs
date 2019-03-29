using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplayObjectDecider : MonoBehaviour
{
    private Camera cam;
    private PlayerOwnedFigurines figs;

    private void Start()
    {
        figs = GetComponent<Transform>().parent.gameObject.GetComponent<PlayerOwnedFigurines>();
    }

    private void CullInvisible(Camera cam)
    {
        GameObject[] npcFigurines = GameObject.FindGameObjectsWithTag("NPCFigurine");
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] fogCells = GameObject.FindGameObjectsWithTag("Fog");

        foreach (GameObject npc in npcFigurines)
        {
            if (IsViewedBy(npc.GetComponent<Figurine_ObservedBy>().GetObservedBy()))
            {
                npc.layer = 0;
            }
            else
            {
                npc.layer = 10;
            }
        }

        foreach (GameObject obs in obstacles)
        {
            if (!IsViewedBy(obs.GetComponent<Obstacle_RevealedTo>().GetObservedBy()))
            {
                obs.layer = 10;
            }
        }

        foreach (GameObject fog in fogCells)
        {
            if (IsViewedBy(fog.GetComponent<FogHideOtherObject>().GetObservedBy()))
            {
                fog.layer = 10;
            }
            else
            {
                fog.layer = 0;
            }
        }
    }

    private void OnPreRender()
    {
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            obs.layer = 9;
        }
    }

    private bool IsViewedBy(GameObject[] visTo)
    {
        foreach (GameObject v in visTo)
        {
            if (figs.ownedFigurine.Equals(v))
            {
                return true;
            }
        }
        return false;
    }

    private void OnEnable()
    {
        Camera.onPreCull += CullInvisible;
    }

    private void OnDisable()
    {
        Camera.onPreCull -= CullInvisible;
    }
}
