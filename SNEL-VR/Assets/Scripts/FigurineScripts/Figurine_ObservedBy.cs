using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figurine_ObservedBy : MonoBehaviour
{
    private List<GameObject> observedBy = new List<GameObject>();

    public void AddObserver(GameObject observer)
    {
        if (!observedBy.Contains(observer))
        {
            observedBy.Add(observer);
        }
    }

    public void RemoveObserver(GameObject observer)
    {
        observedBy.Remove(observer);
    }

    public void ClearObservers()
    {
        observedBy.Clear();
    }

    public GameObject[] GetObservedBy()
    {
        return observedBy.ToArray();
    }
}
