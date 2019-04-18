using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShowMovment : MonoBehaviour
{
    private bool highlighted;

    // Start is called before the first frame update
    void Start()
    {
        highlighted = false;
    }

    // Finds and highlights the the squares within movement range
    public void Highlight()
    {
        GridHandler gridhandler = GameObject.FindWithTag("GridHandler").GetComponent<GridHandler>();
        GameObject figurine = GameObject.FindWithTag("PlayerFigurine");
        HashSet<GameObject> movement = gridhandler.FindSquares(figurine);
        foreach(GameObject square in movement)
        {
            Renderer renderer = square.GetComponent<Renderer>();
            renderer.material.SetColor("_Color", new Color(0, 0xba, 0xbe));
        }
    }
}
