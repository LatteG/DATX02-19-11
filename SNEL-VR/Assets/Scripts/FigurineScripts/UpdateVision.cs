using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVRTouchSample;

public class UpdateVision : MonoBehaviour
{
    private  Figurine_PlayerVision playerVision;
    private OculusSampleFramework.DistanceGrabbable distanceGrabbable;
    private Transform figurineTransform;

    private bool wasGrabbed = false;
    private Vector3 oldPos;

    private void OnEnable()
    {
        distanceGrabbable = GetComponent<OculusSampleFramework.DistanceGrabbable>();
        playerVision = GetComponentInChildren<Figurine_PlayerVision>();
        figurineTransform = GetComponent<Transform>();
        oldPos = figurineTransform.position;
    }

    private void Update()
    {
        bool isGrabbed = distanceGrabbable.isGrabbed;
        Vector3 newPos = figurineTransform.position;

        bool released = !isGrabbed && wasGrabbed;

        if (released || !oldPos.Equals(newPos))
        {
            playerVision.ShouldUpdate();
        }

        oldPos = newPos;
        wasGrabbed = isGrabbed;
    }
}
