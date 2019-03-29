using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogHideOtherObject : MonoBehaviour
{
    private List<Collider> npcColliders = new List<Collider>();
    private List<Collider> obstacleColliders = new List<Collider>();

    private List<GameObject> observedBy = new List<GameObject>();
    private List<GameObject> hasBeenObservedBy = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // What to do when colliding with an NPC figurine.
        if (ColliderHasTag(other, "NPCFigurine"))
        {
            // Reset the figurines observers.
            other.gameObject.GetComponent<Figurine_ObservedBy>().ClearObservers();

            // Make the figurine visible to all figurines currently observing this fog element.
            for (int i = 0; i < observedBy.Count; i++)
            {
                MakeVisible(other, observedBy[i]);
            }
        }
        // What to do when colliding with an obstacle.
        else if (ColliderHasTag(other, "Obstacle"))
        {
            // Make the obstacle visible to all figurines currently observing this fog element.
            for (int i = 0; i < observedBy.Count; i++)
            {
                MakeVisible(other, observedBy[i]);
            }
        }
    }

    // Returns the position of the fog element
    public Vector3 GetPosition()
    {
        return transform.parent.position;
    }

    // Called by a player's vision script when it has line of sight to the cell.
    public void SeenBy(GameObject figurine)
    {
        if (!observedBy.Contains(figurine))
        {
            observedBy.Add(figurine);
            if (!hasBeenObservedBy.Contains(figurine))
            {
                hasBeenObservedBy.Add(figurine);
            }
        }
        RevealContents(figurine);
    }

    // Called by a player's vision script when it no longer has line of sight to the cell.
    public void NotSeenBy(GameObject figurine)
    {
        observedBy.Remove(figurine);
        HideContents(figurine);
    }

    // Reveal any and all objects hidden by this fog cell.
    private void RevealContents(GameObject figurine)
    {
        // Make all NPC figurines visible
        for (int i = 0; i < npcColliders.Count; i++)
        {
            MakeVisible(npcColliders[i], figurine);
        }

        // Make all obstacles visible
        for (int i = 0; i < obstacleColliders.Count; i++)
        {
            MakeVisible(obstacleColliders[i], figurine);
        }
    }

    // Hides all objects tagged with the NPCFigurines-tag that can be hidden by this fog cell.
    private void HideContents(GameObject figurine)
    {
        // Make all NPC figurines visible
        for (int i = 0; i < npcColliders.Count; i++)
        {
            MakeInvisible(npcColliders[i], figurine);
        }
    }

    // Checks if the GameObject holding the collider has the tag.
    private bool ColliderHasTag(Collider other, string tag)
    {
        return other.gameObject.CompareTag(tag);
    }

    private void MakeInvisible(Collider c, GameObject figurine)
    {
        MakeInvisible(c.gameObject, figurine);
    }
    private void MakeInvisible(GameObject go, GameObject figurine)
    {
        if (go.CompareTag("NPCFigurine"))
        {
            go.GetComponent<Figurine_ObservedBy>().RemoveObserver(figurine);
        }
    }

    private void MakeVisible(Collider c, GameObject figurine)
    {
        MakeVisible(c.gameObject, figurine);
    }
    private void MakeVisible(GameObject go, GameObject figurine)
    {
        if (go.CompareTag("NPCFigurine"))
        {
            go.GetComponent<Figurine_ObservedBy>().AddObserver(figurine);
        }
        else if (go.CompareTag("Obstacle"))
        {
            go.GetComponent<Obstacle_RevealedTo>().AddObserver(figurine);
        }
    }

    public GameObject[] GetObservedBy()
    {
        GameObject[] retList = new GameObject[observedBy.Count];
        observedBy.CopyTo(retList);
        return retList;
    }

    public GameObject[] getHasBeenObservedBy()
    {
        GameObject[] retList = new GameObject[hasBeenObservedBy.Count];
        hasBeenObservedBy.CopyTo(retList);
        return retList;
    }
}
