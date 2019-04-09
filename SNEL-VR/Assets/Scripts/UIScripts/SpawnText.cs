using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnText : MonoBehaviour
{
    public Text text;

    public void Text()
    {
        Instantiate(text);
    }
}
