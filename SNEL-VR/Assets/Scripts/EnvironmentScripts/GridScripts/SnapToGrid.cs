using System.Collections;
using System.Collections.Generic;
using OVRTouchSample;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    private GridHandler gridHandler;
    private HashSet<string> snappingTags;

    private void OnEnable()
    {
        gridHandler = transform.parent.gameObject.GetComponent<GridHandler>();

        snappingTags = new HashSet<string>();
        snappingTags.Add("NPCFigurine");
        snappingTags.Add("PlayerFigurine");
    }

    // Tells the gridHandler to snap a fig to the grid when one enters the grid square.
    private void OnTriggerEnter(Collider other)
    {
        if (snappingTags.Contains(other.gameObject.tag))
        {
            gridHandler.AddToSnappingQueue(other.gameObject.transform.parent.gameObject);
        }
    }

    // Zeroes the velocity and angular velocity of any figurine that exits the grid square.
    private void OnTriggerExit(Collider other)
    {
        if (snappingTags.Contains(other.gameObject.tag))
        {
            gridHandler.ZeroVelocity(other.gameObject.transform.parent.gameObject);
        }
    }
}
