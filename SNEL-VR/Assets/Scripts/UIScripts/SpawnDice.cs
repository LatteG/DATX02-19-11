using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDice : MonoBehaviour
{

    public GameObject dice;
    public Transform Spawn;

    HashSet<GameObject> diceToKill = new HashSet<GameObject>();

    public void killDie()
    {
        foreach (GameObject u in diceToKill)
        {
            Debug.Log("Enter loop");
            Destroy(u.gameObject);
            Debug.Log("Destroyed!");
        }

        Debug.Log("Exit loop");
    }
     
    public void spawnDie()
    {
        GameObject die = Instantiate(dice, Spawn.position, Quaternion.Euler(Random.Range(0,360), Random.Range(0, 360), Random.Range(0, 360)) );
        diceToKill.Add(die);
        //diceToKill.Add((GameObject)Instantiate(dice, Spawn.position, Quaternion.identity));
        //diceToKill.Add(die);
    }
}

