using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    Rigidbody rb;

    bool hasLanded;
    bool thrown;

    Vector3 initPosition;

    public int diceValue;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPosition = transform.position;

    }
}
