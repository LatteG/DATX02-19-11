using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float movementSpeed          = 5f;
    public MovementInput inputMethod    = MovementInput.NONE;

    public enum MovementInput {NONE, KEYBOARD};

    public Rigidbody rb;

    // Update is called once per frame
    void Update()
    {
        Vector3 movementVector;

        if (inputMethod == MovementInput.KEYBOARD)
        {
            // If chosen input method is keyboard, the movement vector is set to a normalized vector based on what buttons are held
            movementVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Redirect the vector to be in the same direction as the rigidbody but purely horizontal
            movementVector = rb.transform.TransformDirection(movementVector);
            movementVector.y = 0;
            movementVector = movementVector.normalized;
        }
        else
        {
            // If no input method is chosen the movement vector is set to 0
            movementVector = new Vector3(0, 0, 0);
        }

        // Multiply the movement vector with the movement speed and delta time to get the correct magnitude
        movementVector *= movementSpeed;
        movementVector *= Time.deltaTime;
        
        // Translate the rigidbody with the movement vector to move the player correctly
        rb.transform.Translate(movementVector, Space.World);
    }
}
