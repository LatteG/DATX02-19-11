using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookController : MonoBehaviour
{
    public float lookSensitivity    = 50f;
    public LookInput inputMethod    = LookInput.NONE;

    public enum LookInput {NONE, MOUSE};

    public Rigidbody rb;

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation;
        // Quaternion rotX;
        // Quaternion rotY;

        if (inputMethod == LookInput.MOUSE)
        {
            // Only rotate view when right-cick is held
            if (Input.GetMouseButton(1))
            {
                // Get input to create a vector for delta-angles
                Vector3 mousePos = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
                // Get current viewing angle
                Vector3 origin = rb.rotation.eulerAngles;

                // Change size of delta-angle-vector
                mousePos *= lookSensitivity;
                mousePos *= Time.deltaTime;

                // Add current viewing angles to create target viewing angle
                mousePos += origin;

                // Create a quaternion from the viewing angle
                rotation = Quaternion.Euler(mousePos);
            }
            else
            {
                // Don't do anything if right-click is not held
                return;
            }
        }
        else
        {
            // If no input-method is set, don't do anything
            return;
        }

        // Set viewing angle to a new angle based on input
        rb.MoveRotation(rotation);
    }
}
