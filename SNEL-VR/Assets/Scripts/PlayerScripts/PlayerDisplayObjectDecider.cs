using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplayObjectDecider : MonoBehaviour
{
    public Material unvisitedFogMaterial;
    public Material visitedFogMaterial;
    public GameObject player;

    private PlayerOwnedFigurines figs;

    private HashSet<GameObject> tempKnownObjects;
    private HashSet<GameObject> permKnownObjects;

    private static int defaultLayer = 0;
    private static int invisibleLayer = 15;
    private static int obstacleLayer = 9;

    // Makes any object that should not be visible by this camera invisible by changing their layer
    // right before the culling.
    private void OnPreCull()
    {
        // Get all temporarily and permanently known objects from each figurine owned by the player attached to this camera.
        foreach (GameObject playerFig in figs.GetOwnedFigurines())
        {
            Figurine_PlayerVision playerVis = playerFig.GetComponentInChildren<Figurine_PlayerVision>();

            tempKnownObjects.UnionWith(playerVis.GetTempKnownObjects());
            permKnownObjects.UnionWith(playerVis.GetPermKnownObjects());
        }

        // Makes all permanently known non-fog objects visible.
        foreach (GameObject go in permKnownObjects)
        {
            if (!go.CompareTag("Fog"))
            {
                go.layer = defaultLayer;
            }
        }

        // Makes all temporarily known fog cells invisible and all other temporarily known objects visible.
        foreach (GameObject go in tempKnownObjects)
        {
            if (go.CompareTag("Fog"))
            {
                go.layer = invisibleLayer;
            }
            else
            {
                go.layer = defaultLayer;
            }
        }
    }

    // Updates textures and resets obstacles' layer before rendering.
    private void OnPreRender()
    {
        // Update the material of all fog elements that have at some point been seen by any known figurine.
        // Makes the obstacles invisible again to prevent inadvertent visibility for other cameras.
        foreach (GameObject go in permKnownObjects)
        {
            if (go.CompareTag("Fog"))
            {
                go.GetComponent<Renderer>().material = visitedFogMaterial;
            }
            else
            {
                go.layer = invisibleLayer;
            }
        }
        
        // Makes the fog visible again, and everything else that is only seen when in range invisible again
        // to prevent inadvertant (in)visibility to other cameras.
        foreach (GameObject go in tempKnownObjects)
        {
            if (go.CompareTag("Fog"))
            {
                go.layer = defaultLayer;
            }
            else
            {
                go.layer = invisibleLayer;
            }
        }
    }

    private void OnPostRender()
    {
        // Changes the material of the figurines back to default in case they aren't visited by the other players.
        foreach (GameObject go in permKnownObjects)
        {
            if (go.CompareTag("Fog"))
            {
                go.GetComponent<Renderer>().material = unvisitedFogMaterial;
            }
        }

        // Clears the sets of temporarily and permanently known objects in case some are no longer visible, an owned figurine is lost, etc.
        tempKnownObjects.Clear();
        permKnownObjects.Clear();
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

        tempKnownObjects = new HashSet<GameObject>();
        permKnownObjects = new HashSet<GameObject>();
    }
}
