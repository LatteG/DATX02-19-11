using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Teleport()
    {
        Vector3 pos = transform.position;
        pos.y += 0.1f;
        transform.position = pos;
    }
}
