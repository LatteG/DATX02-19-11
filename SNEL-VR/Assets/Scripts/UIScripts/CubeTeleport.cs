using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTeleport : MonoBehaviour
{
    public void Teleport()
    {
        Vector3 pos = transform.position;
        pos.y += 0.1f;
        transform.position = pos;
    }
}
