using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figurine_PlayerVision : MonoBehaviour
{
    private static float sensitivity = 0.00000001f;

    private bool hasMoved = true;
    private bool shouldUpdate = false;

    private Vector3 oldPos;
    private Vector3 newPos;

    private int obstacleLayerMask;
    private float visionRange;

    // Sets initial values and values that are never changed.
    private void Start()
    {
        // Used for the first FixedUpdate-call.
        oldPos = GetComponent<Transform>().parent.position;

        // Used for raycasts.
        visionRange = GetComponent<SphereCollider>().radius;
        obstacleLayerMask = 1 << 9;
    }

    // Check for movement to determine if the OnCollisionStay-script should fire.
    void FixedUpdate()
    {
        newPos = GetComponent<Transform>().parent.position;

        if (!shouldUpdate)
        {
            // If the script does not allow updates then it should check if it SHOULD allow for updates.

            float dx = Mathf.Abs(newPos.x - oldPos.x);
            // float dy = Mathf.Abs(newPos.y - oldPos.y);
            float dz = Mathf.Abs(newPos.z - oldPos.z);

            // If the figurine has moved but is no longer moving in the horizontal plane then it
            // should allow updates of the fog elements.
            bool movementThisFrame = dx > sensitivity || dz > sensitivity;
            if (hasMoved && !movementThisFrame)
            {
                shouldUpdate = true;
            }

            hasMoved = movementThisFrame;
        }
        else
        {
            // If the script has updated the fog cells since the last movement then it should no
            // longer allow updates.
            shouldUpdate = false;
        }

        oldPos = newPos;
    }

    // Checks if a fog element is visible or not and sends the appropriate call to its script.
    private void OnTriggerStay(Collider other)
    {
        // Does not update anything if the figurine has not moved.
        if (!shouldUpdate)
        {
            return;
        }

        if (ColliderHasTag(other, "Fog"))
        {
            Vector3 fogPos = other.gameObject.GetComponent<FogHideOtherObject>().GetPosition();
            Vector3 sphereCenter = GetComponent<SphereCollider>().center + oldPos;
            Vector3 direction = (fogPos - sphereCenter);
            float raycastRange = direction.magnitude;

            // Checks if there is an obstacle between the figurine and fog element.
            if (Physics.Raycast(sphereCenter, direction, raycastRange, obstacleLayerMask))
            {
                // Is an obstacle.
                // Debug.Log("Wall detected!");
                Debug.DrawLine(sphereCenter, other.gameObject.GetComponent<FogHideOtherObject>().GetPosition(), Color.red);
                // Debug.DrawLine(sphereCenter, sphereCenter + (direction.normalized * raycastRange), Color.red);
                TellOutOfLOS(other);
            }
            else
            {
                // Is no obstacle.
                Debug.DrawLine(sphereCenter, other.gameObject.GetComponent<FogHideOtherObject>().GetPosition(), Color.green);
                // Debug.DrawLine(sphereCenter, sphereCenter + (direction.normalized * raycastRange), Color.green);
                TellInLOS(other);
            }
        }
    }

    // Sends a call to a fog element's script to tell it it is no longer visible when visited.
    private void OnTriggerExit(Collider other)
    {
        if (ColliderHasTag(other, "Fog"))
        {
            TellOutOfLOS(other);
        }
    }

    // Tells a fog element it is no longer visible.
    private void TellOutOfLOS(Collider other)
    {
        other.gameObject.GetComponent<FogHideOtherObject>().NotSeenBy(this.gameObject.transform.parent.gameObject);
    }

    // Tells a fog element it is visible.
    private void TellInLOS(Collider other)
    {
        other.gameObject.GetComponent<FogHideOtherObject>().SeenBy(this.gameObject.transform.parent.gameObject);
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
