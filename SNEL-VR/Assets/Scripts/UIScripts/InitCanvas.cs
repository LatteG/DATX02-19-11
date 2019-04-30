using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitCanvas : MonoBehaviour
{
    private Transform targetTransform;
    private Camera eventCamera;
    private GameObject pointer;
    private void Start()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);

        targetTransform = player;
        eventCamera = Camera.main;
        Debug.Log("Camera: "+eventCamera);
        pointer = GameObject.FindGameObjectWithTag("Laser");

        GetComponent<RotateCanvas>().target = targetTransform;
        GetComponent<Canvas>().worldCamera = eventCamera;
        GetComponent<OVRRaycaster>().pointer = pointer;
    }
}
