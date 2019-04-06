using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_RevealedTo : MonoBehaviour
{
    private HashSet<GameObject> observedBy = new HashSet<GameObject>();

    public void AddObserver(GameObject observer)
    {
        observedBy.Add(observer);
    }

    public HashSet<GameObject> GetObservedBy()
    {
        return observedBy;
    }
}
