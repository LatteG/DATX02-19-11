using System.Collections;
using System.Collections.Generic;
using OVRTouchSample;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    private int rows;
    private int cols;
    private float gridSize;
    private Vector3 scale;

    private Vector3 originPoint;

    private Dictionary<Vector2, GameObject> gridMap;
    private HashSet<GameObject> figurines;

    private void Awake()
    {
        this.enabled = false;
    }

    public void init(int rows, int cols, float gridSize, Vector3 scale)
    {
        this.rows = rows;
        this.cols = cols;
        this.gridSize = gridSize * scale.x;
        this.scale = scale;

        enabled = true;
    }

    private void OnEnable()
    {
        if (rows == -1 || cols == -1 || gridSize <= 0)
        {
            enabled = false;
            return;
        }

        gridMap = new Dictionary<Vector2, GameObject>();
        figurines = new HashSet<GameObject>();
        originPoint = gameObject.GetComponent<Transform>().position - new Vector3(cols * scale.x / 2f, 0, rows * scale.z / 2f);
        
        gameObject.AddComponent<BoxCollider>();
        BoxCollider bc = gameObject.GetComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(cols * gridSize / scale.x, 0.1f, rows * gridSize / scale.x);
        bc.center = new Vector3(-0.5f * gridSize, 0.1f, -0.5f * gridSize);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPCFigurine") || other.gameObject.CompareTag("PlayerFigurine"))
        {
            figurines.Add(other.gameObject.GetComponent<Transform>().parent.gameObject);
            // Debug.Log("A figurine with the tag \"" + other.gameObject.tag + "\" has entered the grid.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPCFigurine") || other.gameObject.CompareTag("PlayerFigurine"))
        {
            figurines.Remove(other.gameObject.GetComponent<Transform>().parent.gameObject);
            ZeroVelocity(other.gameObject.GetComponent<Transform>().parent.gameObject);
            // Debug.Log("A figurine with the tag \"" + other.gameObject.tag + "\" has left the grid.");
        }
    }

    private void FixedUpdate()
    {
        foreach (GameObject fig in figurines)
        {
            if (!fig.GetComponent<OculusSampleFramework.DistanceGrabbable>().isGrabbed)
            {
                if (fig.GetComponent<Transform>().hasChanged)
                {
                    SnapToGrid(fig);
                }
            }
        }
    }

    public void SnapToGrid(GameObject fig)
    {
        Transform figTransform = fig.GetComponent<Transform>();

        // Get the current position and offset it to suit the grid.
        Vector3 oldPos = figTransform.position;
        oldPos.x -= gridSize / 2;
        oldPos.z -= gridSize / 2;

        // Get the new position.
        Vector3 newPos = GetSnapToGridPosition(oldPos);
        newPos.y = oldPos.y;

        // Set the new position.
        figTransform.position = newPos;

        // Reset the angle around the x- and z-axis.
        Quaternion figRot = figTransform.rotation;
        figRot.eulerAngles = Vector3.zero;

        // Set the velocity and angular velocity to zero
        ZeroVelocity(fig);

        figTransform.hasChanged = false;

        Debug.Log("Figurine has been moved from " + oldPos + " to " + newPos);
        Debug.DrawLine(oldPos, newPos, Color.blue, 0.25f);
    }

    private Vector2 PositionToKey(Vector3 pos)
    {
        float x = (pos.x - originPoint.x) / gridSize;
        float y = (pos.z - originPoint.z) / gridSize;

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
        Vector3 pos = value.GetComponent<MeshRenderer>().bounds.min;
        Vector2 key = PositionToKey(pos);
        return AddToMap(value, key);
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

    private void ZeroVelocity(GameObject go)
    {
        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
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
