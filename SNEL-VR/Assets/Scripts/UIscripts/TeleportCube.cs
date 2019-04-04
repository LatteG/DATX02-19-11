using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCube : MonoBehaviour
{
    public void Teleport()
    {
        Vector3 pos = transform.position;
        pos.y += 1;
        transform.position = pos;
    }
}
