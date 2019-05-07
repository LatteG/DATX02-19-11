using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePlayerMenu : MonoBehaviour
{
    public GameObject menu;

    GameObject charactersheet;
    GameObject inventory;
    GameObject attacks;

    bool menuActive;
    bool sheetActive = true;
    bool inventoryActive = true;
    bool attacksActive = true;


    void Start()
    {
        menuActive = false;
        charactersheet = GameObject.Find("CharacterSheet");
        inventory = GameObject.Find("Inventory");
        attacks = GameObject.Find("Attacks");
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            menu.SetActive(!menuActive);
            menuActive = !menuActive;
        }
    }

    public void EnableCharacterSheet()
    {
        charactersheet.SetActive(sheetActive);
        sheetActive = !sheetActive;
    }

    public void EnableInventory()
    {
        inventory.SetActive(inventoryActive);
        inventoryActive = !inventoryActive;
    }

    public void EnableAttacks()
    {
        attacks.SetActive(attacksActive);
        attacksActive = !attacksActive;
    }
}
