using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figurine_PlayerVision : MonoBehaviour
{
    private bool shouldUpdate = false;
    private bool hasUpdated = false;

    private Vector3 pos;
    private Vector3 scale;
    private Vector3 sphereCenter;

    private int obstacleLayerMask;
    private float visionRange;

    private Transform parentTransform;
    private GameObject figurine;

    private HashSet<Collider> fogOrFigExitingRange;
    private HashSet<GameObject> fogCellsInRange;
    private HashSet<GameObject> knownFogCells;
    private HashSet<GameObject> figsInRange;

    // Called whenever the fog should be updated from the figurine's perspective.
    public void ShouldUpdate()
    {
        shouldUpdate = true;
    }

    // Sets initial values and values that are never changed.
    private void OnEnable()
    {
        parentTransform = GetComponent<Transform>().parent;
        scale = GetComponent<Transform>().lossyScale;

        // Used for the first FixedUpdate-call.
        pos = parentTransform.position;

        // Used for raycasts.
        visionRange = GetComponent<SphereCollider>().radius * scale.x * 0.95f;
        sphereCenter = GetComponent<SphereCollider>().center;
        sphereCenter.x *= scale.x;
        sphereCenter.y *= scale.y;
        sphereCenter.z *= scale.z;
        obstacleLayerMask = 1 << 9;

        // Used for LoS-updates in fog elements.
        figurine = parentTransform.gameObject;

        fogOrFigExitingRange = new HashSet<Collider>();
        fogCellsInRange = new HashSet<GameObject>();
        knownFogCells = new HashSet<GameObject>();
        figsInRange = new HashSet<GameObject>();

        foreach (Transform child in parentTransform)
        {
            if (child.gameObject.CompareTag("PlayerFigurine"))
            {
                figsInRange.Add(child.gameObject);
                break;
            }
        }
    }

    // Updates the position and checks if the script has updated the fog.
    void FixedUpdate()
    {
        pos = parentTransform.position;
        if (hasUpdated)
        {
            foreach (Collider c in fogOrFigExitingRange)
            {
                OnTriggerStay(c);
            }
            fogOrFigExitingRange.Clear();

            shouldUpdate = false;
            hasUpdated = false;
        }
    }

    // Checks if a collider should have its visibility sttus updated.
    private void OnTriggerStay(Collider other)
    {
        // Does not update anything if the figurine has not moved.
        if (!shouldUpdate)
        {
            return;
        }
        hasUpdated = true;

        if (ColliderHasTag(other, "Fog") || ColliderHasTag(other, "NPCFigurine") || ColliderHasTag(other, "PlayerFigurine"))
        {
            UpdateColliderStatus(other);
        }
    }

    // Checks if the collider belongs to an object that should be visible to this figurine's owner.
    // Returns true if the object is within the visibility range, and false if it is outside it.
    public bool UpdateColliderStatus(Collider other)
    {
        Vector3 otherPos = other.gameObject.GetComponent<Transform>().position;
        Vector3 direction = otherPos - (sphereCenter + pos);
        float raycastRange = direction.magnitude;

        // Checks if the other object is out of range.
        if (raycastRange > visionRange)
        {
            TellOutOfLOS(other.gameObject);
            return false;
        }
        else
        {
            // Checks if there is an obstacle between the figurine and fog element.
            if (Physics.Raycast(sphereCenter + pos, direction, raycastRange, obstacleLayerMask))
            {
                // Is an obstacle.
                Debug.DrawLine(sphereCenter + pos, otherPos, Color.red);
                TellOutOfLOS(other.gameObject);
            }
            else
            {
                // Is no obstacle.
                Debug.DrawLine(sphereCenter + pos, otherPos, Color.green);
                TellInLOS(other.gameObject);
            }
            return true;
        }
    }

    // Sends a call to a fog element's script to tell it it is no longer visible when visited.
    private void OnTriggerExit(Collider other)
    {
        if (ColliderHasTag(other, "Fog") || ColliderHasTag(other, "NPCFigurine") || ColliderHasTag(other, "PlayerFigurine"))
        {
            fogOrFigExitingRange.Add(other);
        }
    }

    // Tells a fog element it is no longer visible.
    private void TellOutOfLOS(GameObject other)
    {
        if (other.CompareTag("Fog"))
        {
            other.GetComponent<FogHideOtherObject>().NotSeenBy(figurine);

            fogCellsInRange.Remove(other);
        }
        else
        {
            figsInRange.Remove(other);
        }
    }

    // Tells a fog element it is visible.
    private void TellInLOS(GameObject other)
    {
        if (other.CompareTag("Fog"))
        {
            other.GetComponent<FogHideOtherObject>().SeenBy(figurine);

            fogCellsInRange.Add(other);
            knownFogCells.Add(other);
        }
        else
        {
            figsInRange.Add(other);
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

    public HashSet<GameObject> GetFogCellsInRange()
    {
        return fogCellsInRange;
    }

    public HashSet<GameObject> GetKnownFogCells()
    {
        return knownFogCells;
    }

    public HashSet<GameObject> GetFigsInRange()
    {
        return figsInRange;
    }
}
