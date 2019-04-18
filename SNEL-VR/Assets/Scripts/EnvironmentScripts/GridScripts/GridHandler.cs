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
    private HashSet<GameObject> figurinesToBeSnapped;
    private HashSet<GameObject> snappedFigurines;

    private void Awake()
    {
        this.enabled = false;
    }

    // Initializes and enables the script.
    public void init(int rows, int cols, float gridSize, Vector3 scale)
    {
        this.rows = rows;
        this.cols = cols;
        this.gridSize = gridSize * scale.x;
        this.scale = scale;

        enabled = true;
    }

    // Checks that the script should be enabled and then initializes some variables.
    private void OnEnable()
    {
        if (rows == -1 || cols == -1 || gridSize <= 0)
        {
            enabled = false;
            return;
        }

        gridMap = new Dictionary<Vector2, GameObject>();
        figurinesToBeSnapped = new HashSet<GameObject>();
        snappedFigurines = new HashSet<GameObject>();
        originPoint = gameObject.GetComponent<Transform>().position - new Vector3(cols * scale.x / 2f, 0, rows * scale.z / 2f);
    }

    private void FixedUpdate()
    {
        // Snaps figurines to the grid unless they are being held.
        foreach (GameObject fig in figurinesToBeSnapped)
        {
            if (!fig.GetComponent<OculusSampleFramework.DistanceGrabbable>().isGrabbed)
            {
                SnapToGrid(fig);
                snappedFigurines.Add(fig);

                Figurine_UpdatePlayerVision ffupv = fig.GetComponent<Figurine_UpdatePlayerVision>();

                if (ffupv != null)
                {
                    ffupv.ExternalUpdateCall();
                }
            }
        }

        // Removes the ones that were snapped from the figurinesToBeSnapped-HashSet.
        figurinesToBeSnapped.ExceptWith(snappedFigurines);
        snappedFigurines.Clear();
    }

    private void SnapToGrid(GameObject fig)
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

        // Debug.Log("Figurine has been moved from " + oldPos + " to " + newPos);
        Debug.DrawLine(oldPos, newPos, Color.blue, 0.25f);
    }

    // Translates a position to a key but does not check for validity.
    private Vector2 PositionToKey(Vector3 pos)
    {
        float x = (pos.x - originPoint.x) / gridSize;
        float y = (pos.z - originPoint.z) / gridSize;

        return new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }

    // Checks if a key is within the valid intervals.
    private bool IsValidKey(Vector2 key)
    {
        return key.x >= 0 
            && key.x < cols 
            && key.y >= 0 
            && key.y < rows;
    }

    // Adds a game object to the Dictionary that keeps track on all grid squares.
    // Returns false if the position leads to an invalid key.
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

    // Adds a figurine that should be snapped to a grid position.
    public void AddToSnappingQueue(GameObject fig)
    {
        figurinesToBeSnapped.Add(fig);
    }

    // Zeroes the velocity and angular velocity of the Rigidbody attached to the GameObject.
    public void ZeroVelocity(GameObject go)
    {
        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
    }

    // Returns the position of the center of the grid square that pos is in, returns pos if pos is outside of all grid squares.
    private Vector3 GetSnapToGridPosition(Vector3 pos)
    {
        Vector2 key = PositionToKey(pos);
        if (!IsValidKey(key))
        {
            return pos;
        }

        return gridMap[key].GetComponent<Transform>().position;
    }

    // Returns all squares within a set amount of steps from the figurine
    public HashSet<GameObject> FindSquares(GameObject fig)
    {
        Transform figTransform = fig.GetComponent<Transform>();
        Vector3 pos = figTransform.position;
        int steps = 3;
        HashSet<Vector2> inRange = FindInRange(steps, pos);
        if (inRange == null)
        {
            return null;
        }
        HashSet<GameObject> squares = new HashSet<GameObject>();
        foreach(Vector2 u in inRange)
        {
            squares.Add(gridMap[u]);
        }
        return squares;
    }

    // Returns the positions of all squares a set amount of steps away from a given start position
    private HashSet<Vector2> FindInRange(int steps, Vector3 pos)
    {
        Vector2 gridPos = PositionToKey(pos);
        Debug.Log(gridPos);
        if(!IsValidKey(gridPos))
        {
            return null;
        }
        HashSet<Vector2> start = new HashSet<Vector2>();
        start.Add(gridPos);
        return FindInRange(steps, start);
    }

    // Returns the positions of all squares a set amount of steps away from a set of already found squares
    private HashSet<Vector2> FindInRange(int steps, HashSet<Vector2> current)
    {
        HashSet<Vector2> next = new HashSet<Vector2>();
        foreach(Vector2 u in current)
        {
            next.UnionWith(GetNeighbours(u));
        }
        if(steps <= 1)
        {
            current.UnionWith(next);
        }
        else
        {
            next.ExceptWith(current);
            current.UnionWith(FindInRange(--steps, next));
        }

        return current;

    }

    // Returns the positions of the squares one step away from a given position
    private HashSet<Vector2> GetNeighbours(Vector2 key)
    {
        HashSet<Vector2> retSet = new HashSet<Vector2>();

        Vector2 temp = new Vector2(key.x + 1, key.y);
        if (IsValidKey(temp))
        {
            retSet.Add(temp);
        }
        temp = new Vector2(key.x - 1, key.y);
        if (IsValidKey(temp))
        {
            retSet.Add(temp);
        }
        temp = new Vector2(key.x, key.y + 1);
        if (IsValidKey(temp))
        {
            retSet.Add(temp);
        }
        temp = new Vector2(key.x, key.y - 1);
        if (IsValidKey(temp))
        {
            retSet.Add(temp);
        }

        return retSet;
    }
}

