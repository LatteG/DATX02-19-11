using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_RevealedTo : MonoBehaviour
{
    private List<GameObject> observedBy = new List<GameObject>();

    public void AddObserver(GameObject observer)
    {
        if (!observedBy.Contains(observer))
        {
            observedBy.Add(observer);
        }
    }

    public GameObject[] GetObservedBy()
    {
        GameObject[] retList = new GameObject[observedBy.Count];
        observedBy.CopyTo(retList);
        return retList;
    }
}
