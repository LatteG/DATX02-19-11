using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShowMovment : MonoBehaviour
{
    public int steps;

    private bool highlighted;
    private HashSet<GameObject> movement;

    // Start is called before the first frame update
    void Start()
    {
        highlighted = false;
        movement = new HashSet<GameObject>();
    }

    // Finds and highlights the the squares within movement range
    public void Highlight()
    {
        if (!highlighted)
        {
            GridHandler gridhandler = GameObject.FindWithTag("GridHandler").GetComponent<GridHandler>();
            GameObject figurine = GameObject.FindWithTag("PlayerFigurine");
            movement = gridhandler.FindSquares(figurine, steps);
            foreach (GameObject square in movement)
            {
                Renderer renderer = square.GetComponent<Renderer>();
                renderer.material.SetColor("_Color", new Color(0, 0xba, 0xbe));
            }
            highlighted = true;
        }
        else
        {
            foreach (GameObject square in movement)
            {
                Renderer renderer = square.GetComponent<Renderer>();
                renderer.material.SetColor("_Color", Color.white);
            }
            highlighted = false;
        }
    }
}
