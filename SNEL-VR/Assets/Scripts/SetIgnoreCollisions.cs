using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetIgnoreCollisions : MonoBehaviour
{
    public LayerMask baseLayer;
    public List<LayerMask> layersToCollideWith;

    private void OnEnable()
    {
        // Saves the numbers of the layers to ignore.
        List<int> layersToCollideWithInts = layersToInts(layersToCollideWith);

        // Saves the number of the base layer.
        int baseLayerInt = layerToInt(baseLayer);

        // Loops through all possible layers to set the base layer to ignore all that
        // are not chosen to be collided with.
        for (int layer = 0; layer < 32; layer++)
        {
            // Tells the physics engine that there should be no collisions between the
            // base layer and "current" layer if the "current" layer was not chosen.
            if (!layersToCollideWithInts.Contains(layer))
            {
                Physics.IgnoreLayerCollision(baseLayerInt, layer, true);
            }
        }
    }

    // Converts a list of LayerMasks to a list of the layer-numbers.
    private List<int> layersToInts(List<LayerMask> inputList)
    {
        List<int> retList = new List<int>();
        foreach (LayerMask lm in inputList)
        {
            retList.Add(layerToInt(lm));
        }
        return retList;
    }

    // Converts a LayerMask to its layer's number.
    private int layerToInt(LayerMask lm)
    {
        return (int)Mathf.Log(lm, 2);
    }
}
