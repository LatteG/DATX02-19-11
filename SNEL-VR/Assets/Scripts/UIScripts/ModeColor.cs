﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeColor : MonoBehaviour
{
    Material m_Material;
    private GameObject ModeObject;
    //TESTprivate Light glow;

    public Material material;
        // ex. Color AttackColor = Color.yellow;

    private bool enabledMenu = false;

    private void Start()
    {
        ModeObject = transform.parent.parent.transform.GetChild(0).gameObject;
        //TESTglow = GetComponent<Light>();
    }

    public void ColorChange()
    {
        //Debug.Log("Nu trycktes knappen ner.");
        ModeObject.GetComponent<Renderer>().sharedMaterial = material;
        //TESTglow.enabled = !glow.enabled;
    }

}
