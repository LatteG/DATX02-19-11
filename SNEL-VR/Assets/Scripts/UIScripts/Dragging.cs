using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragging : MonoBehaviour
{
    public GameObject player;

    private bool enabledDrag;
    private Vector3 pos;

    private Vector3 posOffset = new Vector3(1, 0, 0);
    private float floatDistance = 1.0f;

    public void func()
    {
        enabledDrag = !enabledDrag;
    }

    private void Start()
    {
        pos = transform.position;
        enabledDrag = false;
    }

    public void Update()
    {
        pos = player.transform.position + posOffset;
    }
}
