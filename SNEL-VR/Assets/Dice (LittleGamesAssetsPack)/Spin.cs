// Rotate an object around its Y (upward) axis in response to
// left/right controls.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float torque;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddRelativeTorque(Vector3.up * torque);
    }
}
