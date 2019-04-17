using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figurine_ObservedBy : MonoBehaviour
{
    private HashSet<GameObject> observedBy = new HashSet<GameObject>();

    public void AddObserver(GameObject observer)
    {
        observedBy.Add(observer);
    }

    public void RemoveObserver(GameObject observer)
    {
        observedBy.Remove(observer);
    }

    public void ClearObservers()
    {
        observedBy.Clear();
    }

    public HashSet<GameObject> GetObservedBy()
    {
        return observedBy;
    }
}
