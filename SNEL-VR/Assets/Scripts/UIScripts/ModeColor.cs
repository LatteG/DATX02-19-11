using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeColor : MonoBehaviour
{
    Material m_Material;
    public Renderer ModeObject;
    public Material AttackMaterial;
    public Material ActionMaterial;
    // ex. Color AttackColor = Color.yellow;

    private void Start()
    {
        Material rend = ModeObject.GetComponent<Renderer>().material;
    }

    public void ColorChange()
    {
        //Debug.Log("Nu trycktes knappen ner.");
        ModeObject.sharedMaterial = AttackMaterial;
    }

    public void ColorChange2()
    {
        //Debug.Log("Nu trycktes knappen ner.");
        ModeObject.sharedMaterial = ActionMaterial;
    }

}
