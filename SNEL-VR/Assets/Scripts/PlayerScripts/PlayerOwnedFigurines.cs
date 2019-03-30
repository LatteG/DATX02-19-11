using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOwnedFigurines : MonoBehaviour
{
    // This should probably be removed later, but it is good for debugging purposes.
    public GameObject[] inputFigurines;
    private List<GameObject> ownedFigurines = new List<GameObject>();

    private void Start()
    {
        foreach (GameObject fig in inputFigurines)
        {
            AddFigurine(fig);
        }
    }

    public void AddFigurine(GameObject fig)
    {
        if (!ownedFigurines.Contains(fig))
        {
            ownedFigurines.Add(fig);
        }
    }

    public bool RemoveFigurine(GameObject fig)
    {
        return ownedFigurines.Remove(fig);
    }

    public GameObject[] GetOwnedFigurines()
    {
        // GameObject[] retList = new GameObject[ownedFigurines.Count];
        // ownedFigurines.CopyTo(retList);
        return ownedFigurines.ToArray();
    }
}
