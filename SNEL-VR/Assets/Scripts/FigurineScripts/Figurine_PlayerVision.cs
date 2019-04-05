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

    private List<Collider> fogCellsExitingRange;

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

        fogCellsExitingRange = new List<Collider>();
    }

    // Updates the position and checks if the script has updated the fog.
    void FixedUpdate()
    {
        pos = parentTransform.position;
        if (hasUpdated)
        {
            foreach (Collider c in fogCellsExitingRange)
            {
                OnTriggerStay(c);
            }
            fogCellsExitingRange.Clear();

            shouldUpdate = false;
            hasUpdated = false;
        }
    }

    // Checks if a fog element is visible or not and sends the appropriate call to its script.
    private void OnTriggerStay(Collider other)
    {
        // Does not update anything if the figurine has not moved.
        if (!shouldUpdate)
        {
            return;
        }
        hasUpdated = true;

        if (ColliderHasTag(other, "Fog"))
        {
            Vector3 fogPos = other.gameObject.GetComponent<FogHideOtherObject>().GetPosition();
            Vector3 direction = fogPos - (sphereCenter + pos);
            float raycastRange = direction.magnitude;

            // Checks if the fog element is out of range.
            if (raycastRange > visionRange)
            {
                TellOutOfLOS(other);
            }
            else
            {
                // Checks if there is an obstacle between the figurine and fog element.
                if (Physics.Raycast(sphereCenter + pos, direction, raycastRange, obstacleLayerMask))
                {
                    // Is an obstacle.
                    Debug.DrawLine(sphereCenter + pos, other.gameObject.GetComponent<FogHideOtherObject>().GetPosition(), Color.red);
                    TellOutOfLOS(other);
                }
                else
                {
                    // Is no obstacle.
                    Debug.DrawLine(sphereCenter + pos, other.gameObject.GetComponent<FogHideOtherObject>().GetPosition(), Color.green);
                    TellInLOS(other);
                }
            }
        }
    }

    // Sends a call to a fog element's script to tell it it is no longer visible when visited.
    private void OnTriggerExit(Collider other)
    {
        if (ColliderHasTag(other, "Fog"))
        {
            // TellOutOfLOS(other);
            if (!fogCellsExitingRange.Contains(other))
            {
                fogCellsExitingRange.Add(other);
            }
        }
    }

    // Tells a fog element it is no longer visible.
    private void TellOutOfLOS(Collider other)
    {
        other.gameObject.GetComponent<FogHideOtherObject>().NotSeenBy(figurine);
    }

    // Tells a fog element it is visible.
    private void TellInLOS(Collider other)
    {
        other.gameObject.GetComponent<FogHideOtherObject>().SeenBy(figurine);
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
}
