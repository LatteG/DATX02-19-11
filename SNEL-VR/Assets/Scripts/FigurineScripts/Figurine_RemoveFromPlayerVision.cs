using System.Collections;
using System.Collections.Generic;
using OVRTouchSample;
using UnityEngine;

public class Figurine_RemoveFromPlayerVision : MonoBehaviour
{
    private OculusSampleFramework.DistanceGrabbable distanceGrabbable;
    private HashSet<Figurine_PlayerVision> observers;
    private Transform parentTransform;
    private GameObject figurine;
    private Collider thisCollider;
    private Vector3 oldPos;

    private bool hasMoved = false;
    private float sensitivity = 0.00001f;

    private void OnEnable()
    {
        observers = new HashSet<Figurine_PlayerVision>();

        figurine = this.gameObject;
        parentTransform = figurine.GetComponent<Transform>().parent;
        thisCollider = figurine.GetComponent<Collider>();
        distanceGrabbable = parentTransform.gameObject.GetComponent<OculusSampleFramework.DistanceGrabbable>();

        oldPos = parentTransform.position;
    }

    private void FixedUpdate()
    {
        Vector3 newPos = parentTransform.position;
        bool justMoved = (oldPos - newPos).sqrMagnitude > sensitivity;
        bool hasStopped = hasMoved && !justMoved;

        // Checks if th figurine has stopped moving and is not being held.
        if (hasStopped && !distanceGrabbable.isGrabbed)
        {
            HashSet<Figurine_PlayerVision> toBeRemoved = new HashSet<Figurine_PlayerVision>();

            // Updates the visibility status of this figurine for each player figurine observing it.
            // Stops checking scripts of figurines that are too far away.
            foreach (Figurine_PlayerVision obs in observers)
            {
                if (!obs.UpdateColliderStatus(thisCollider))
                {
                    toBeRemoved.Add(obs);
                }
            }

            observers.ExceptWith(toBeRemoved);
        }

        hasMoved = justMoved;
        oldPos = newPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerFigurineVision"))
        {
            observers.Add(other.GetComponent<Figurine_PlayerVision>());
        }
    }
}
