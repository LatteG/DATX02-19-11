using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplayObjectDecider : MonoBehaviour
{
    public Material unvisitedFogMaterial;
    public Material visitedFogMaterial;
    public GameObject player;

    private PlayerOwnedFigurines figs;

    private HashSet<GameObject> invisibleFogCells;
    private HashSet<GameObject> visitedFogCells;

    private static int defaultLayer = 0;
    private static int invisibleLayer = 15;
    private static int obstacleLayer = 9;

    // Makes any object that should not be visible by this camera invisible by changing their layer
    // right before the culling.
    private void OnPreCull()
    {
        // Put all NPC figurines visible by any owned figurines in the default layer and the rest
        // in the invisible layer.
        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPCFigurine"))
        {
            if (OwnedIsIn(npc.GetComponent<Figurine_ObservedBy>().GetObservedBy()))
            {
                npc.layer = defaultLayer;
            }
            else
            {
                npc.layer = invisibleLayer;
            }
        }

        // Put all obstacles that are not visible to any owned figurines in the invisible layer.
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            if (!OwnedIsIn(obs.GetComponent<Obstacle_RevealedTo>().GetObservedBy()))
            {
                obs.layer = invisibleLayer;
            }
        }

        // Put all fog elements visible by any owned figurines in the default layer and the rest
        // in the invisible layer.
        foreach (GameObject playerFig in figs.GetOwnedFigurines())
        {
            invisibleFogCells.UnionWith(playerFig.GetComponentInChildren<Figurine_PlayerVision>().GetFogCellsInRange());
            visitedFogCells.UnionWith(playerFig.GetComponentInChildren<Figurine_PlayerVision>().GetKnownFogCells());
        }
        foreach (GameObject fog in invisibleFogCells)
        {
            fog.layer = invisibleLayer;
        }


        // Puts all player figurines owned by the player or visible to figurines owned by the
        // player in the default layer and the rest in the invisible layer.
        foreach (GameObject fig in GameObject.FindGameObjectsWithTag("PlayerFigurine"))
        {
            if (IsOwned(fig.transform.parent.gameObject) || OwnedIsIn(fig.GetComponent<Figurine_ObservedBy>().GetObservedBy()))
            {
                fig.layer = defaultLayer;
            }
            else
            {
                fig.layer = invisibleLayer;
            }
        }
    }

    // Updates textures and resets obstacles' layer before rendering.
    private void OnPreRender()
    {
        // Put all obstacles in the obstacle layer.
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            obs.layer = obstacleLayer;
        }

        // Update the material of all fog elements based on whether the owned figurines have
        // at some points seen them or not.
        foreach (GameObject fog in visitedFogCells)
        {
            fog.GetComponent<Renderer>().material = visitedFogMaterial;
        }
    }

    private void OnPostRender()
    {
        foreach (GameObject fog in visitedFogCells)
        {
            fog.GetComponent<Renderer>().material = unvisitedFogMaterial;
            fog.layer = defaultLayer;
        }

        invisibleFogCells.Clear();
        visitedFogCells.Clear();
    }

    // Checks if any object in the input array is owned by the player.
    private bool OwnedIsIn(HashSet<GameObject> visTo)
    {
        foreach (GameObject v in visTo)
        {
            if (IsOwned(v))
            {
                return true;
            }
        }
        return false;
    }

    // Checks if the input object is owned by the player.
    private bool IsOwned(GameObject obj)
    {
        return figs.GetOwnedFigurines().Contains(obj);
    }

    // Adds CullInvisible to OnPreCull and get the ownedFigurines-script when enabled.
    private void OnEnable()
    {
        // figs = GetComponent<Transform>().parent.gameObject.GetComponent<PlayerOwnedFigurines>();
        figs = player.GetComponent<PlayerOwnedFigurines>();
        visitedFogCells = new HashSet<GameObject>();
        invisibleFogCells = new HashSet<GameObject>();
    }
}
