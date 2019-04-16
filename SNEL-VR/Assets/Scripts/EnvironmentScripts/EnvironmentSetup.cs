using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSetup : MonoBehaviour
{
    public float cellSizeHor = 0.1f;
    public float cellSizeVer = 1f;
    public float gridSize = 1f;
    public float gridLineWidth = 0.005f;

    public bool enableSnapToGrid = true;

    public GameObject fogElement;
    public GameObject gridLine;
    public GameObject gridSquare;

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

        SetupFog(meshMin, meshSize, new Vector3(cellSizeHor, cellSizeVer, cellSizeHor), scale);

        // Increase the y-axis to make the grid spawn on top of the table instead of in it.
        meshMin.y += 0.005f;

        // Offset the meshMin to align the grid with the fog
        Vector3 boundsScale = this.GetComponentInChildren<MeshCollider>().gameObject.GetComponent<Transform>().localScale;
        meshMin -= setupTransform.position;
        meshMin.x -= (1 - scale.x) * boundsScale.x / 2;
        meshMin.z -= (1 - scale.z) * boundsScale.y / 2;

        SetupGrid(meshMin, meshSize, scale);
    }

    private void SetupFog(Vector3 areaStart, Vector3 areaSize, Vector3 cellSize, Vector3 scale)
    {
        int cols = (int)(areaSize.x / cellSizeHor);
        int rows = (int)(areaSize.z / cellSizeHor);

        // Sets up the container for the fog elements
        GameObject fogContainer = new GameObject("FogContainer");
        Transform fogContainerTransform = fogContainer.GetComponent<Transform>();
        SetupContainerTransform(fogContainerTransform);

        // Fill the area defined by the MeshCollider with fog elements.
        for (int i = 0; i < cols; i++)
        {
            // Save the value of x for a column.
            float x = areaStart.x + (i + 0.5f) * scale.x * cellSizeHor;
            for (int k = 0; k < rows; k++)
            {
                // Make a vector for the position of the fog element.
                Vector3 pos = new Vector3(x, areaStart.y, areaStart.z + (k + 0.5f) * scale.z * cellSizeHor);
                GameObject fe = Instantiate(fogElement, pos, Quaternion.identity, fogContainerTransform);

                // Resize new fog element.
                fe.transform.localScale = cellSize;
            }
        }
    }

    private void SetupGrid(Vector3 areaStart, Vector3 areaSize, Vector3 scale)
    {
        int cols = (int)(areaSize.x / gridSize);
        int rows = (int)(areaSize.z / gridSize);

        // Sets up the container for the grid lines
        GameObject gridLineContainer = new GameObject("GridLineContainer");
        Transform gridLineContainerTransform = gridLineContainer.GetComponent<Transform>();
        SetupContainerTransform(gridLineContainerTransform);

        
        // Sets up the container for the grid squares
        GameObject gridSquareContainer = new GameObject("GridSquareContainer");

        Transform gridSquareContainerTransform = gridSquareContainer.GetComponent<Transform>();
        SetupContainerTransform(gridSquareContainerTransform);


        // Spawn the vertical lines.
        for (int i = 0; i <= cols; i++)
        {
            // Prepare the positions for the vertices in the line.
            float linePosX = areaStart.x + i * gridSize;
            Vector3[] positions = { new Vector3(linePosX, areaStart.y, areaStart.z),
                                    new Vector3(linePosX, areaStart.y, areaStart.z + areaSize.z) };

            SpawnGridLine(positions, scale.x, gridLineContainerTransform);
        }

        // Spawn the horizontal lines.
        for (int i = 0; i <= rows; i++)
        {
            float linePosZ = areaStart.z + i * gridSize;
            Vector3[] positions = { new Vector3(areaStart.x, areaStart.y, linePosZ),
                                    new Vector3(areaStart.x + areaSize.x, areaStart.y, linePosZ) };

            SpawnGridLine(positions, scale.y, gridLineContainerTransform);
        }

        areaStart.x += 0.5f * gridSize;
        areaStart.y -= 0.0025f;
        areaStart.z += 0.5f * gridSize;

        gridSquareContainer.AddComponent<GridHandler>();
        GridHandler gridSquareContainerGridHandler = gridSquareContainer.GetComponent<GridHandler>();
        gridSquareContainerGridHandler.init(rows, cols, gridSize, scale);

        int added = 0;
        int notAdded = 0;

        // Create the grid slots.
        for (int x = 0; x < cols; x++)
        {
            float xCoord = areaStart.x + x * gridSize;

            for (int z = 0; z < rows; z++)
            {
                float zCoord = areaStart.z + z * gridSize;
                Vector3 pos = new Vector3(xCoord, areaStart.y, zCoord);

                GameObject gs = Instantiate(gridSquare, gridSquareContainerTransform);
                gs.GetComponent<Transform>().localPosition = pos;

                if (enableSnapToGrid)
                {
                    gs.GetComponent<SnapToGrid>().enabled = true;
                }

                if (gridSquareContainerGridHandler.AddToMap(gs))
                {
                    added++;
                }
                else
                {
                    notAdded++;
                    Destroy(gs);
                }
            }
        }

        Debug.Log("Added " + added + " grid squares and failed to add " + notAdded + " grid squares.");
    }

    private void SpawnGridLine(Vector3[] positions, float scale, Transform parentTransform)
    {
        // Spawn the line and set its vertices.
        GameObject line = Instantiate(gridLine, parentTransform);
        LineRenderer lineRen = line.GetComponent<LineRenderer>();

        lineRen.SetPositions(positions);

        lineRen.startWidth = gridLineWidth * scale;
        lineRen.endWidth = gridLineWidth * scale;
    }

    private void SetupContainerTransform(Transform t)
    {
        t.SetParent(setupTransform);
        t.localPosition = new Vector3(0, 0, 0);
        t.localRotation = Quaternion.identity;
        t.localScale = new Vector3(1, 1, 1);
    }
}
