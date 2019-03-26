using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogHideOtherObject : MonoBehaviour
{
    private bool isRevealed = false;
    private bool hasBeenRevealed = false;

    private List<Collider> npcColliders = new List<Collider>();
    private List<Collider> obstacleColliders = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        // If the player figurine has this cell in its range the cell should remember that it is currently being viewed and has been viewed.
        // Also reveals anything it conceals.
        if (ColliderHasTag(other, "PlayerFigurineVision"))
        {
            isRevealed = true;
            hasBeenRevealed = true;
            MakeInvisible(this.gameObject);
            RevealContents();
        }
        // What to do when colliding with an NPC figurine.
        else if (ColliderHasTag(other, "NPCFigurine"))
        {
            // Hide if the cell is not hidden.
            if (!isRevealed)
            {
                MakeInvisible(other);
            }

            // Add NPC figurine to list of NPC figurines contained in the cell.
            if (!npcColliders.Contains(other))
            {
                npcColliders.Add(other);
            }
        }
        // What to do when colliding with an obstacle.
        else if (ColliderHasTag(other, "Obstacle"))
        {
            // Obstacles can only be invisible when the cell has not been previously revealed
            if (!hasBeenRevealed)
            {
                MakeInvisible(other);

                // Since the objects can only be made visible before the cell has been revealed
                // this can be in the body of the hasBeenRevealed-check
                if (!obstacleColliders.Contains(other))
                {
                    obstacleColliders.Add(other);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player figurine no longer has this cell in its range the cell should switch back to not being revealed.
        // The cell should also once again hide all NPC figurines.
        if (ColliderHasTag(other, "PlayerFigurineVision"))
        {
            isRevealed = false;
            MakeVisible(this.gameObject);
            HideContents();
        }
        // Make objects leaving the cell visible.
        else
        {
            MakeVisible(other);

            // Remove NPC figurines that leave the cell from the list of contained NPC figurines.
            if (ColliderHasTag(other, "NPCFigurine"))
            {
                if (npcColliders.Contains(other))
                {
                    npcColliders.Remove(other);
                }
            }
        }
    }

    // Reveal any and all objects hidden by this fog cell.
    private void RevealContents()
    {
        // Make all NPC figurines visible
        for (int i = 0; i < npcColliders.Count; i++)
        {
            MakeVisible(npcColliders[i]);
        }

        // Make all obstacles visible
        for (int i = 0; i < obstacleColliders.Count; i++)
        {
            MakeVisible(obstacleColliders[i]);
        }
    }

    // Hides all objects tagged with the NPCFigurines-tag that can be hidden by this fog cell.
    private void HideContents()
    {
        // Make all NPC figurines visible
        for (int i = 0; i < npcColliders.Count; i++)
        {
            MakeInvisible(npcColliders[i]);
        }
    }

    // Checks if the other collider has the wanted tag.
    // If the collider is untagged it checks the tag of its gameObject.
    // If that gameObject is untagged it checks if that gameObjects parent-gameObject has the tag.
    // Returns false if any of the tags checked is neither "Untagged" nor tag, or if all are "Untagged", true otherwise.
    private bool ColliderHasTag(Collider other, string tag)
    {
        if (other.CompareTag(tag))
        {
            return true;
        }
        else if (other.CompareTag("Untagged"))
        {
            if (other.gameObject.CompareTag(tag))
            {
                return true;
            }
            else if (other.gameObject.CompareTag("Untagged"))
            {
                return other.gameObject.transform.parent.gameObject.CompareTag(tag);
            }
        }

        return false;
    }

    private void MakeInvisible(Collider c)
    {
        MakeInvisible(c.gameObject);
    }
    private void MakeInvisible(GameObject go)
    {
        go.GetComponent<MeshRenderer>().enabled = false;
    }

    private void MakeVisible(Collider c)
    {
        MakeVisible(c.gameObject);
    }
    private void MakeVisible(GameObject go)
    {
        go.GetComponent<MeshRenderer>().enabled = true;
    }
}
