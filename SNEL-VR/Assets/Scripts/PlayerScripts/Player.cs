using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private Vector3 pos;
    private Quaternion rotation;
    private int ID;
    public string name;

    public void InitPlayer(Vector3 pos, Quaternion rotation, int ID, string name)
    {
        this.pos = pos;
        this.rotation = rotation;
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

    public void SetPlayerPos(Vector3 position)
    {
        this.pos = position;
    }

    public void SetPlayerRotation(Quaternion rotation)
    {
        this.rotation = rotation;
    }

    internal Quaternion GetPlayerRotation()
    {
        return this.rotation;
    }
}
