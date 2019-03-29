using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplayObjectDecider : MonoBehaviour
{
    public Material unvisitedFogMaterial;
    public Material visitedFogMaterial;

    private Camera cam;
    private PlayerOwnedFigurines figs;

    private void Start()
    {
        figs = GetComponent<Transform>().parent.gameObject.GetComponent<PlayerOwnedFigurines>();
    }

    private void CullInvisible(Camera cam)
    {
        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPCFigurine"))
        {
            if (OwnedIsIn(npc.GetComponent<Figurine_ObservedBy>().GetObservedBy()))
            {
                npc.layer = 0;
            }
            else
            {
                npc.layer = 10;
            }
        }

        foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            if (!OwnedIsIn(obs.GetComponent<Obstacle_RevealedTo>().GetObservedBy()))
            {
                obs.layer = 10;
            }
        }

        foreach (GameObject fog in GameObject.FindGameObjectsWithTag("Fog"))
        {
            if (OwnedIsIn(fog.GetComponent<FogHideOtherObject>().GetObservedBy()))
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

        foreach (GameObject fog in GameObject.FindGameObjectsWithTag("Fog"))
        {
            if (OwnedIsIn(fog.GetComponent<FogHideOtherObject>().getHasBeenObservedBy()))
            {
                fog.GetComponent<Renderer>().material = visitedFogMaterial;
            }
            else
            {
                fog.GetComponent<Renderer>().material = unvisitedFogMaterial;
            }
        }
    }

    private bool OwnedIsIn(GameObject[] visTo)
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
