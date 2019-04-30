using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This complete script can be attached to a camera to make it
// continuously point at another object.

// The target variable shows up as a property in the inspector.
// Drag another object onto it to make the camera look at it.

public class RotateCanvas : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        // Rotate the camera every frame so it keeps looking at the target
        //transform.LookAt(target);
        //transform.Rotate(0, 180, 0, Space.Self);

        //Vector3 targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
        Vector3 targetPosition = new Vector3(target.position.x, 0f, target.position.z);
        transform.LookAt(targetPosition);
        transform.Rotate(45, 180, 0, Space.Self);
    }
}
