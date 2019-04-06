using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOwnedFigurines : MonoBehaviour
{
    // This should probably be removed later, but it is good for debugging purposes.
    public GameObject[] inputFigurines;
    private HashSet<GameObject> ownedFigurines = new HashSet<GameObject>();

    private void Start()
    {
        foreach (GameObject fig in inputFigurines)
        {
            AddFigurine(fig);
        }
    }

    public void AddFigurine(GameObject fig)
    {
        ownedFigurines.Add(fig);
    }

    public bool RemoveFigurine(GameObject fig)
    {
        return ownedFigurines.Remove(fig);
    }

    public HashSet<GameObject> GetOwnedFigurines()
    {
        return ownedFigurines;
    }
}
