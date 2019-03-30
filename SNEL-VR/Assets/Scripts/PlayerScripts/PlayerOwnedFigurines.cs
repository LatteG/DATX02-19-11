using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOwnedFigurines : MonoBehaviour
{
    public GameObject ownedFigurine;

    public GameObject[] GetOwnedFigurines()
    {
        GameObject[] retArr = { ownedFigurine };
        return retArr;
    }
}
