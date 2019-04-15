using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogHideOtherObject : MonoBehaviour
{
    private HashSet<Collider> figurineColliders = new HashSet<Collider>();
    private HashSet<Collider> obstacleColliders = new HashSet<Collider>();

    private HashSet<GameObject> observedBy = new HashSet<GameObject>();
    private HashSet<GameObject> hasBeenObservedBy = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // What to do when colliding with a figurine.
        if (ColliderHasTag(other, "NPCFigurine") || ColliderHasTag(other, "PlayerFigurine"))
        {
            // Reset the figurines observers.
            other.gameObject.GetComponent<Figurine_ObservedBy>().ClearObservers();

            // Make the figurine visible to all figurines currently observing this fog element.
            foreach (GameObject fig in observedBy)
            {
                MakeVisible(other, fig);
            }
            
            figurineColliders.Add(other);
        }
        // What to do when colliding with an obstacle.
        else if (ColliderHasTag(other, "Obstacle"))
        {
            // Make the obstacle visible to all figurines currently observing this fog element.
            foreach (GameObject fig in observedBy)
            {
                MakeVisible(other, fig);
            }

            obstacleColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ColliderHasTag(other, "NPCFigurine") || ColliderHasTag(other, "PlayerFigurine"))
        {
            figurineColliders.Remove(other);
        }
        else if (ColliderHasTag(other, "Obstacle"))
        {
            obstacleColliders.Remove(other);
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
        observedBy.Add(figurine);
        hasBeenObservedBy.Add(figurine);
        RevealContents(figurine);
    }

    // Called by a player's vision script when it no longer has line of sight to the cell.
    public void NotSeenBy(GameObject figurine)
    {
        observedBy.Remove(figurine);
        HideContents(figurine);
    }

    // Reveal any and all objects hidden by this fog cell.
    private void RevealContents(GameObject playerFig)
    {
        // Make all NPC figurines visible
        foreach (Collider fig in figurineColliders)
        {
            MakeVisible(fig, playerFig);
        }

        // Make all obstacles visible
        foreach (Collider obstacle in obstacleColliders)
        {
            MakeVisible(obstacle, playerFig);
        }
    }

    // Hides all objects tagged with the NPCFigurines-tag that can be hidden by this fog cell.
    private void HideContents(GameObject playerFig)
    {
        // Make all NPC figurines visible
        foreach (Collider fig in figurineColliders)
        {
            MakeInvisible(fig, playerFig);
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
        if (go.CompareTag("NPCFigurine") || go.CompareTag("PlayerFigurine"))
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
        if (go.CompareTag("NPCFigurine") || go.CompareTag("PlayerFigurine"))
        {
            go.GetComponent<Figurine_ObservedBy>().AddObserver(figurine);
        }
        else if (go.CompareTag("Obstacle"))
        {
            go.GetComponent<Obstacle_RevealedTo>().AddObserver(figurine);
        }
    }

    public HashSet<GameObject> GetObservedBy()
    {
        return observedBy;
    }

    public HashSet<GameObject> getHasBeenObservedBy()
    {
        return hasBeenObservedBy;
    }
}
