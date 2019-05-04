using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterBuildOnTriggerExit : MonoBehaviour
{
    public GameMasterBuildManager gmbm;
    private List<GameObject> contains;
    public void Start()
    {
        contains = new List<GameObject>();
    }

    public void OnTriggerEnter(Collider other)
    {
        contains.Add(other.gameObject);
        Debug.Log("Added: " + other);
        Debug.Log("CountContains: " + contains.Count);
    }

    public void OnTriggerExit(Collider other)
    {
        contains.Remove(other.gameObject);
    }
    public void DestroyAll()
    {
        foreach (GameObject go in contains)
        {
            Destroy(go.gameObject);
        }

        contains.Clear();
        contains = new List<GameObject>();
    }

}
