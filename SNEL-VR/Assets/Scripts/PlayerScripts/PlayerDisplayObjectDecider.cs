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
    private HashSet<GameObject> visibleFigs;
    private HashSet<GameObject> knownObstacles;

    private static int defaultLayer = 0;
    private static int invisibleLayer = 15;
    private static int obstacleLayer = 9;

    // Makes any object that should not be visible by this camera invisible by changing their layer
    // right before the culling.
    private void OnPreCull()
    {
        // Put all obstacles that are not visible to any owned figurines in the invisible layer.
        /*foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            if (!OwnedIsIn(obs.GetComponent<Obstacle_RevealedTo>().GetObservedBy()))
            {
                obs.layer = invisibleLayer;
            }
        }*/

        // Get all fog cells that are in range of the owned figurines, the fog cells that have been in range
        // of the owned figurines, the figurines visible to the owned figurines, and the obstacles known to
        // the owned figurines.
        foreach (GameObject playerFig in figs.GetOwnedFigurines())
        {
            Figurine_PlayerVision playerVis = playerFig.GetComponentInChildren<Figurine_PlayerVision>();

            invisibleFogCells.UnionWith(playerVis.GetFogCellsInRange());
            visitedFogCells.UnionWith(playerVis.GetKnownFogCells());
            visibleFigs.UnionWith(playerVis.GetFigsInRange());
            knownObstacles.UnionWith(playerVis.GetKnownObstacles());
        }

        // Makes the fog cells in range invisible.
        foreach (GameObject fog in invisibleFogCells)
        {
            fog.layer = invisibleLayer;
        }

        // Makes the figs in range visible.
        foreach (GameObject fig in visibleFigs)
        {
            fig.layer = defaultLayer;
        }

        // Makes the known obstacles visible.
        foreach (GameObject obs in knownObstacles)
        {
            obs.layer = defaultLayer;
        }
    }

    // Updates textures and resets obstacles' layer before rendering.
    private void OnPreRender()
    {
        // Update the material of all fog elements based on whether the owned figurines have
        // at some points seen them or not.
        foreach (GameObject fog in visitedFogCells)
        {
            fog.GetComponent<Renderer>().material = visitedFogMaterial;
        }

        // Makes the fog cells visible again to prevent inadvertent visibility for other cameras.
        foreach (GameObject fog in invisibleFogCells)
        {
            fog.layer = defaultLayer;
        }

        // Makes the figurines invisible again to prevent inadvertent visibility for other cameras.
        foreach (GameObject fig in visibleFigs)
        {
            fig.layer = invisibleLayer;
        }

        // Makes the obstacles invisible again to prevent inadvertent visibility for other cameras.
        foreach (GameObject obs in knownObstacles)
        {
            obs.layer = invisibleLayer;
        }
    }

    private void OnPostRender()
    {
        // Changes the material of the figurines back to default in case they aren't visited by the other players.
        foreach (GameObject fog in visitedFogCells)
        {
            fog.GetComponent<Renderer>().material = unvisitedFogMaterial;
        }

        // Clears the sets of fog cells and figurines in case some are no longer visible, an owned figurine is lost, etc.
        invisibleFogCells.Clear();
        visitedFogCells.Clear();
        visibleFigs.Clear();
        knownObstacles.Clear();
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
        visibleFigs = new HashSet<GameObject>();
        knownObstacles = new HashSet<GameObject>();
    }
}
