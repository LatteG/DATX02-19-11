using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDice : MonoBehaviour
{
    public GameObject dice;
    public Transform Spawn;

    private List<GameObject> diceList;

    private void OnEnable()
    {
        diceList = new List<GameObject>();
    }

    public void killDice()
    {
        Debug.Log("diceList contains " + diceList.Count + " dice");

        for (int i = diceList.Count - 1; i >= 0; i--)
        {
            Destroy(diceList[i]);
            diceList.RemoveAt(i);
            Debug.Log("Destroyed a dice");
        }
    }
     
    public void spawnDie()
    {
        GameObject die = Instantiate(dice, Spawn.position, Quaternion.Euler(Random.Range(0,360), Random.Range(0, 360), Random.Range(0, 360)) );
        diceList.Add(die);
        //diceToKill.Add((GameObject)Instantiate(dice, Spawn.position, Quaternion.identity));
        //diceToKill.Add(die);
    }
}

