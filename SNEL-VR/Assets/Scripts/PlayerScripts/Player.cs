using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 pos;
    private int ID;
    public string name;

    public void InitPlayer(Vector3 pos, int ID, string name)
    {
        this.pos = pos;
        this.ID = ID;
        this.name = name;
    }

    public void PlayerUpdatePos(Vector3 newPos)
    {
        pos = newPos;
    }

    public Vector3 GetPlayerPos()
    {
        return this.pos;
    }

    public int GetPlayerID()
    {
        return ID;
    }

    internal void SetPlayerPos(Vector3 position)
    {
        this.pos = position;
    }
}
