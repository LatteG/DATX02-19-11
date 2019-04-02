using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreCubes : MonoBehaviour
{
    public GameObject cubePrefab;
    public Vector3 spawn = new Vector3(15, 5, 0);

    public void Cube()
    {
        Instantiate(cubePrefab, spawn, Quaternion.identity);
    }
}
