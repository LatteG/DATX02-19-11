using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragging : MonoBehaviour
{
    Vector3 playerPos;
    Vector3 myPos;
    int count = 0;

    private void Start()
    {
        myPos = transform.position;
    }

    private void Update()
    {
        playerPos = GameObject.FindObjectOfType<OVRPlayerController>().transform.position;
        transform.position = playerPos + Vector3.forward * 2;
        count++;
        if(count > 100)
        {
            Debug.Log(playerPos);
            count = 0;
        }
    }
}

