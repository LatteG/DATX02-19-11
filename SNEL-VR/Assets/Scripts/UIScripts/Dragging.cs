using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragging : MonoBehaviour
{
    public GameObject anchor;
    public Vector3 start;

    Vector3 playerPos;
    Vector3 unitPosOffest;

    bool enabledDragging;

    private void Start()
    {
        unitPosOffest = start;
        enabledDragging = false;
        transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (enabledDragging)
        {
            unitPosOffest = anchor.transform.forward;

            playerPos = GameObject.FindObjectOfType<OVRPlayerController>().transform.position;
            transform.parent.position = new Vector3(playerPos.x, playerPos.y + 0.5f, playerPos.z) + unitPosOffest * 0.75f;
        }
        else
        {
            playerPos = GameObject.FindObjectOfType<OVRPlayerController>().transform.position;
            transform.parent.position = new Vector3(playerPos.x, playerPos.y + 0.5f, playerPos.z) + unitPosOffest * 0.75f;
        }      
    }

    public void Drag()
    {
        enabledDragging = !enabledDragging;
        Debug.Log("pointer down");
    }

}

