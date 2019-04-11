using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSetup : MonoBehaviour
{
    public float cellSizeHor = 0.1f;
    public float cellSizeVer = 1f;
    public float gridSize = 1f;
    public float gridWidth = 0.005f;

    public GameObject fogElement;
    public GameObject gridLine;

    private Transform setupTransform;
    
    private void OnEnable()
    {
        setupTransform = this.gameObject.GetComponent<Transform>();

        // Get size of the mesh called Boundary in the child.
        Vector3 meshSize = GetComponentInChildren<MeshCollider>().bounds.size;

        //To correct size/pos contra scale
        Vector3 scale = setupTransform.lossyScale;

        meshSize.x /= scale.x;
        meshSize.z /= scale.z;

        // Get the starting points.
        Vector3 meshMin = GetComponentInChildren<MeshCollider>().bounds.min;

        // We have no more use for this.
        GetComponentInChildren<MeshCollider>().enabled = false;

        SpawnFog(meshMin, meshSize, new Vector3(cellSizeHor, cellSizeVer, cellSizeHor), scale);

        // Increase the y-axis to make the grid lines spawn on top of the fog instad of under it.
        meshMin.y += cellSizeVer * 1.05f;

        // Offset the meshMin to align the grid with the fog
        Vector3 boundsScale = this.GetComponentInChildren<MeshCollider>().gameObject.GetComponent<Transform>().localScale;
        meshMin -= setupTransform.position;
        meshMin.x -= (1 - scale.x) * boundsScale.x / 2;
        meshMin.z -= (1 - scale.z) * boundsScale.y / 2;

        SpawnGridLines(meshMin, meshSize, scale);
    }

    private void SpawnFog(Vector3 areaStart, Vector3 areaSize, Vector3 cellSize, Vector3 scale)
    {
        int cols = (int)(areaSize.x / cellSizeHor);
        int rows = (int)(areaSize.z / cellSizeHor);

        // Fill the area defined by the MeshCollider with fog elements.
        for (int i = 0; i < cols; i++)
        {
            // Save the value of x for a column.
            float x = areaStart.x + (i + 0.5f) * scale.x * cellSizeHor;
            for (int k = 0; k < rows; k++)
            {
                // Make a vector for the position of the fog element.
                Vector3 pos = new Vector3(x, areaStart.y, areaStart.z + (k + 0.5f) * scale.z * cellSizeHor);
                GameObject fe = Instantiate(fogElement, pos, Quaternion.identity, setupTransform);

                // Resize new fog element.
                fe.transform.localScale = cellSize;
            }
        }
    }

    private void SpawnGridLines(Vector3 areaStart, Vector3 areaSize, Vector3 scale)
    {
        int cols = (int)(areaSize.x / gridSize) + 1;
        int rows = (int)(areaSize.z / gridSize) + 1;

        // Spawn the vertical lines
        for (int i = 0; i < cols; i++)
        {
            // Prepare the positions for the vertices in the line.
            float linePosX = areaStart.x + i * gridSize;
            Vector3[] positions = { new Vector3(linePosX, areaStart.y, areaStart.z),
                                    new Vector3(linePosX, areaStart.y, areaStart.z + areaSize.z) };

            SpawnGridLine(positions, scale.x);
        }

        // Spawn the horizontal lines
        for (int i = 0; i < rows; i++)
        {
            float linePosZ = areaStart.z + i * gridSize;
            Vector3[] positions = { new Vector3(areaStart.x, areaStart.y, linePosZ),
                                    new Vector3(areaStart.x + areaSize.x, areaStart.y, linePosZ) };

            SpawnGridLine(positions, scale.y);
        }
    }

    private void SpawnGridLine(Vector3[] positions, float scale)
    {
        // Spawn the line and set its vertices.
        GameObject line = Instantiate(gridLine, setupTransform);
        LineRenderer lineRen = line.GetComponent<LineRenderer>();

        lineRen.SetPositions(positions);

        lineRen.startWidth = gridWidth * scale;
        lineRen.endWidth = gridWidth * scale;

        Debug.Log("Spawns line between " + positions[0] + " and " + positions[1]);
    }
}
