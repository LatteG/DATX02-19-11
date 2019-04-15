using System.Collections;
using System.Collections.Generic;
using OVRTouchSample;
using UnityEngine;

public class Figurine_UpdatePlayerVision : MonoBehaviour
{
    private Figurine_PlayerVision playerVision;
    private OculusSampleFramework.DistanceGrabbable distanceGrabbable;
    private Transform figTransform;

    private Vector3 oldPos;
    private float sensitivity = 0.00001f;

    private bool awaitStandstill = false;
    private bool wasGrabbed = false;

    public int waitForFrames = 3;
    private int waitedFrames = 0;

    // Gets the components to be called, makes one initial update not to make the figurine stand in fog.
    private void OnEnable()
    {
        playerVision = GetComponentInChildren<Figurine_PlayerVision>();
        distanceGrabbable = GetComponent<OculusSampleFramework.DistanceGrabbable>();
        figTransform = GetComponent<Transform>();

        playerVision.ShouldUpdate();
    }

    // Checks if the figurine's vision should be updated.
    private void FixedUpdate()
    {
        // Checks if the figurine has been released and is waiting until it has stopped moving.
        if (awaitStandstill)
        {
            // Checks if the figurines have waited enough frames.
            if (++waitedFrames > waitForFrames)
            {
                Vector3 newPos = figTransform.position;

                // Checks if the figurine is standing still enough.
                if ((oldPos - newPos).sqrMagnitude < sensitivity)
                {
                    // Updates the figurine's vision.
                    awaitStandstill = false;
                    playerVision.ShouldUpdate();
                }
                else
                {
                    oldPos = newPos;
                }
            }
        }
        else
        {
            // Checks if the figurine was just released.
            bool isGrabbed = distanceGrabbable.isGrabbed;
            bool justReleased = wasGrabbed && !isGrabbed;
            if (justReleased)
            {
                // Prepares for awaiting a standstill to make sure the figurine updates the right piece of fog.
                awaitStandstill = true;
                oldPos = figTransform.position;
                waitedFrames = 0;
                wasGrabbed = false;
            }
            else
            {
                wasGrabbed = isGrabbed;
            }
        }
    }
}
