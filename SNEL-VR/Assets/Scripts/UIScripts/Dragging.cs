using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragging : MonoBehaviour
{
    public GameObject anchor;

    Vector3 playerPos;
    Vector3 myPos;
    Vector3 unitPosOffest;

    bool enabledDragging;

    private void Start()
    {
        myPos = transform.parent.position;
        unitPosOffest = Vector3.forward;
        enabledDragging = false;
    }

    private void Update()
    {

        if (enabledDragging)
        {
            unitPosOffest = anchor.transform.forward;

            playerPos = GameObject.FindObjectOfType<OVRPlayerController>().transform.position;
            transform.parent.position = new Vector3(playerPos.x, playerPos.y + 0.5f, playerPos.z) + unitPosOffest * 0.5f;
        }
        else
        {
            playerPos = GameObject.FindObjectOfType<OVRPlayerController>().transform.position;
            transform.parent.position = new Vector3(playerPos.x, playerPos.y + 0.5f, playerPos.z) + unitPosOffest * 0.5f;
        }
    }

    public void Drag()
    {
        enabledDragging = !enabledDragging;
        Debug.Log("pointer down");
    }
}

