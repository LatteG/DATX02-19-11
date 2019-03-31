using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSpawner : MonoBehaviour
{
    public float cellSizeHor = 0.1f;
    public float cellSizeVer = 1f;
    public GameObject fogElement;
    
    void Start()
    {
        // Get size of the mesh called Boundary in the child.
        Vector3 meshSize = GetComponentInChildren<MeshCollider>().bounds.size;
        float sizeX = meshSize.x;
        float sizeZ = meshSize.z;

        // Get the starting points.
        Vector3 meshMin = GetComponentInChildren<MeshCollider>().bounds.min;
        float minX = meshMin.x;
        float minZ = meshMin.z;

        // Gets the y-coordinate, assumes an even surface with no change in the y-axis.
        float posY = meshMin.y;

        // We have no more use for this.
        GetComponentInChildren<MeshCollider>().enabled = false;

        int rows = (int) (sizeX / cellSizeHor - cellSizeHor / 2);
        int cols = (int) (sizeZ / cellSizeHor - cellSizeHor / 2);
        Vector3 cellSizeVector = new Vector3(cellSizeHor, cellSizeVer, cellSizeHor);

        // Fill the area defined by the MeshCollider with fog elements.
        for (int i = 0; i < rows; i++)
        {
            // Save the value of x for a column.
            float x = minX + (i + 0.5f) * cellSizeHor;
            for (int k = 0; k < cols; k++)
            {
                // Make a vector for the position of the fog element.
                Vector3 pos = new Vector3(x, posY, minZ + (k + 0.5f) * cellSizeHor);
                GameObject fe = Instantiate(fogElement, pos, Quaternion.identity, this.transform);

                // Resize new fog element.
                fe.transform.localScale = cellSizeVector;
            }
        }
    }
}
