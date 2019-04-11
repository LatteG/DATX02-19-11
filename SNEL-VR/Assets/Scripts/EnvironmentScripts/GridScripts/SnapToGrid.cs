using System.Collections;
using System.Collections.Generic;
using OVRTouchSample;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    private Vector3 gridCenter;
    private HashSet<Collider> collidersToBeProcessed;

    private void OnEnable()
    {
        gridCenter = this.gameObject.GetComponent<MeshRenderer>().bounds.center;
        collidersToBeProcessed = new HashSet<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        collidersToBeProcessed.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        collidersToBeProcessed.Remove(other);
    }

    private void FixedUpdate()
    {
        foreach (Collider c in collidersToBeProcessed)
        {
            ProcessCollider(c);
        }
    }

    private void ProcessCollider(Collider other)
    {
        if (other.gameObject.tag.Contains("Figurine") && !other.gameObject.CompareTag("PlayerFigurineVision"))
        {
            Transform figTransform = other.gameObject.GetComponent<Transform>().parent;

            if (!figTransform.gameObject.GetComponent<OculusSampleFramework.DistanceGrabbable>().isGrabbed)
            {
                MoveObjectToGridCenter(figTransform);

                if (other.gameObject.CompareTag("PlayerFigurine"))
                {
                    figTransform.gameObject.GetComponentInChildren<Figurine_PlayerVision>().ShouldUpdate();
                }

                collidersToBeProcessed.Remove(other);
            }
        }
        else
        {
            collidersToBeProcessed.Remove(other);
        }
    }

    private void MoveObjectToGridCenter(Transform t)
    {
        // Sets the position in the xz-plane while maintaining the y-position.
        Vector3 newPos = new Vector3(gridCenter.x, t.position.y, gridCenter.z);
        t.position = newPos;

        // Sets the rotation around the x- and z-axis to 0 while maintaining the rotation around the y-axis.
        Quaternion newRot = new Quaternion
        {
            eulerAngles = new Vector3(0, t.rotation.eulerAngles.y, 0)
        };
    }
}
