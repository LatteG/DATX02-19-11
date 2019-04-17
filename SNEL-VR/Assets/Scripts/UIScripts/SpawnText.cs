using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnText : MonoBehaviour
{
    public Text text;
    private Vector3 pos = new Vector3(0, 50, 0);

    public void Text()
    {
        Instantiate(text, pos, Quaternion.identity);
    }

}
