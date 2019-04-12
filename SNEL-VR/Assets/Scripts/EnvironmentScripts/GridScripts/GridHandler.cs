using System.Collections;
using System.Collections.Generic;
using OVRTouchSample;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    private int rows;
    private int cols;
    private float gridSize;
    private Vector3 originPoint;

    private Dictionary<Vector2, GameObject> gridMap;
    private HashSet<GameObject> figurines;

    private void Awake()
    {
        this.enabled = false;
    }

    public void init(int rows, int cols, float gridSize, Vector3 originPoint)
    {
        this.rows = rows;
        this.cols = cols;
        this.gridSize = gridSize;
        this.originPoint = originPoint;

        this.enabled = true;
    }

    private void OnEnable()
    {
        if (rows == -1 || cols == -1 || gridSize <= 0)
        {
            this.enabled = false;
            return;
        }

        gridMap = new Dictionary<Vector2, GameObject>();

        GameObject gsc = this.gameObject;
        gsc.AddComponent<BoxCollider>();
        BoxCollider bc = gsc.GetComponent<BoxCollider>();
        bc.size = new Vector3(cols * gridSize, 0.1f, rows * gridSize);
        bc.center = new Vector3(0, 0.1f, 0);
        bc.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPCFigurine") || other.gameObject.CompareTag("PlayerFigurine"))
        {
            figurines.Add(other.gameObject.GetComponent<Transform>().parent.gameObject);
        }
    }

    private void FixedUpdate()
    {
        foreach (GameObject fig in figurines)
        {
            if (!fig.GetComponent<OculusSampleFramework.DistanceGrabbable>().isGrabbed)
            {
                Transform figTransform = fig.GetComponent<Transform>();
                figTransform.position = GetSnapToGridPosition(figTransform.position);
            }
        }
    }

    private Vector2 PositionToKey(Vector3 pos)
    {
        float x = (pos.x - originPoint.x) / gridSize;
        float y = (pos.y - originPoint.y) / gridSize;

        return new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }

    private bool IsValidKey(Vector2 key)
    {
        return key.x >= 0 
            && key.x < cols 
            && key.y >= 0 
            && key.y < rows;
    }

    public bool AddToMap(GameObject value)
    {
        return AddToMap(value, PositionToKey(value.GetComponent<Transform>().position));
    }
    public bool AddToMap(GameObject value, Vector2 key)
    {
        if (!IsValidKey(key))
        {
            return false;
        }

        if (gridMap.ContainsKey(key))
        {
            return false;
        }

        gridMap.Add(key, value);
        return true;
    }

    public Vector3 GetSnapToGridPosition(Vector3 pos)
    {
        Vector2 key = PositionToKey(pos);
        if (!IsValidKey(key))
        {
            return pos;
        }

        return gridMap[key].GetComponent<Transform>().position;
    }
}
