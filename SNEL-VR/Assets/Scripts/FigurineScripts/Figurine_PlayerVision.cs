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

    private HashSet<Collider> tempKnownObjectExitingRange;

    private HashSet<GameObject> permKnownObjects;
    private HashSet<GameObject> tempKnownObjects;

    private HashSet<GameObject> obstaclesInRange;
    private HashSet<Collider> collidersToBeProcessed;

    private LineOfSightCalculator loscalc;

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

        tempKnownObjectExitingRange = new HashSet<Collider>();

        permKnownObjects = new HashSet<GameObject>();
        tempKnownObjects = new HashSet<GameObject>();
        obstaclesInRange = new HashSet<GameObject>();
        collidersToBeProcessed = new HashSet<Collider>();

        loscalc = new LineOfSightCalculator();

        // Adds the attached figurine to the set of permanently known objects.
        foreach (Transform child in parentTransform)
        {
            if (child.gameObject.CompareTag("PlayerFigurine"))
            {
                permKnownObjects.Add(child.gameObject);
                break;
            }
        }
    }

    // Updates the position and checks if the script has updated the fog.
    void FixedUpdate()
    {
        loscalc.CalculateLOS(pos, visionRange, obstaclesInRange);
        loscalc.DebugDrawTriangles(pos.y);

        obstaclesInRange.Clear();

        pos = parentTransform.position;
        return;
        if (hasUpdated)
        {
            foreach (Collider c in tempKnownObjectExitingRange)
            {
                OnTriggerStay(c);
            }
            tempKnownObjectExitingRange.Clear();

            shouldUpdate = false;
            hasUpdated = false;
        }
    }

    // Checks if a collider should have its visibility sttus updated.
    private void OnTriggerStay(Collider other)
    {
        // Does not update anything if the figurine has not moved.
        /*if (!shouldUpdate)
        {
            return;
        }
        hasUpdated = true;*/

        if (ColliderHasTag(other, "Fog") || ColliderHasTag(other, "NPCFigurine") || ColliderHasTag(other, "PlayerFigurine"))
        {
            // UpdateColliderStatus(other);
            collidersToBeProcessed.Add(other);
        }
        else if (ColliderHasTag(other, "Obstacle"))
        {
            // UpdateObstacleColliderStatus(other);
            collidersToBeProcessed.Add(other);
            obstaclesInRange.Add(other.gameObject);
        }
    }

    // Checks if the collider belongs to an object that should be visible to this figurine's owner.
    // Returns true if the object is within the visibility range, and false if it is outside it.
    public bool UpdateColliderStatus(Collider other)
    {
        Vector3 direction = other.gameObject.GetComponent<Transform>().position - (sphereCenter + pos);
        float raycastRange = direction.magnitude;

        // Checks if the other object is out of range.
        if (raycastRange > visionRange)
        {
            TellOutOfLOS(other.gameObject);
            return false;
        }
        else
        {
            DoLOSRaycast(other, direction, raycastRange);
            return true;
        }
    }

    public bool UpdateObstacleColliderStatus(Collider other)
    {
        // Make sure the collider belongs to an obstacle.
        if (!ColliderHasTag(other, "Obstacle"))
        {
            return false;
        }

        // No need to do any more calls if the obstacle is already known.
        if (permKnownObjects.Contains(other.gameObject))
        {
            return true;
        }

        Transform obstacleTransform = other.gameObject.GetComponent<Transform>().parent;
        Vector3 obstaclePos = obstacleTransform.position;
        Vector3 direction = obstaclePos - (sphereCenter + pos);
        float raycastRange = direction.magnitude;

        // Makes sure the object is in range.
        if (raycastRange > visionRange)
        {
            return false;
        }
        else
        {
            // Gets the blocking part of of the object other belongs to.
            GameObject blocker = null;
            foreach (Transform child in obstacleTransform)
            {
                if (child.gameObject.layer == (int) Mathf.Log(obstacleLayerMask, 2))
                {
                    blocker = child.gameObject;
                    break;
                }
            }

            // Makes sure that blocker is assigned some value.
            if (blocker == null)
            {
                return false;
            }

            // Disables the blocking part of the obstacle to prevent it from being "in the way" of itself.
            blocker.layer = 0;
            
            DoLOSRaycast(other, direction, raycastRange);

            // Enables the blocking part of the obstacle to allow it to block LOS to other objects.
            blocker.layer = (int)Mathf.Log(obstacleLayerMask, 2);

            return true;
        }
    }

    // Calls the raycast and appropriate method based on its result.
    private void DoLOSRaycast(Collider other, Vector3 direction, float raycastRange)
    {
        // Checks if there is an obstacle between the figurine and fog element.
        if (Physics.Raycast(sphereCenter + pos, direction, raycastRange, obstacleLayerMask))
        {
            // Is an obstacle.
            Debug.DrawLine(sphereCenter + pos, other.gameObject.GetComponent<Transform>().position, Color.red, 1);
            TellOutOfLOS(other.gameObject);
        }
        else
        {
            // Is no obstacle.
            Debug.DrawLine(sphereCenter + pos, other.gameObject.GetComponent<Transform>().position, Color.green, 1);
            TellInLOS(other.gameObject);
        }
    }

    // Sends a call to a fog element's script to tell it it is no longer visible when visited.
    private void OnTriggerExit(Collider other)
    {
        if (ColliderHasTag(other, "Fog") || ColliderHasTag(other, "NPCFigurine") || ColliderHasTag(other, "PlayerFigurine"))
        {
            tempKnownObjectExitingRange.Add(other);
        }
    }

    // Tells a fog element it is no longer visible.
    private void TellOutOfLOS(GameObject other)
    {
        if (!other.CompareTag("Obstacle"))
        {
            tempKnownObjects.Remove(other);
        }
    }

    // Tells a fog element it is visible.
    private void TellInLOS(GameObject other)
    {
        if (other.CompareTag("Fog"))
        {
            permKnownObjects.Add(other);
            tempKnownObjects.Add(other);
        }
        else if (other.CompareTag("Obstacle"))
        {
            permKnownObjects.Add(other);
        }
        else
        {
            tempKnownObjects.Add(other);
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

    public HashSet<GameObject> GetTempKnownObjects()
    {
        return tempKnownObjects;
    }

    public HashSet<GameObject> GetPermKnownObjects()
    {
        return permKnownObjects;
    }
}
