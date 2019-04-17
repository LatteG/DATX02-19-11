using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private Vector3 pos;
    private Quaternion rotation;
    private int ID;
    private GameObject avatar;
    private HashSet<GameObject> ownedFigurines;

    public string name;

    public void InitPlayer(Vector3 pos, Quaternion rotation, int ID, string name)
    {
        this.pos = pos;
        this.rotation = rotation;
        this.ID = ID;
        this.name = name;
        this.ownedFigurines = new HashSet<GameObject>();

        SetAvatar();
    }

    private void SetAvatar()
    {
        Vector3 pos_avatar = new Vector3(this.pos.x, 1f, this.pos.z);
        avatar = GameObject.Instantiate(GameObject.FindGameObjectWithTag("Avatar"), pos_avatar, this.rotation) as GameObject;
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
    public void ShowAvatar()
    {
        this.avatar.SetActive(true);
    }

    public void HideAvatar()
    {
        this.avatar.SetActive(false);
    }

    public void MoveAvatar()
    {
        Vector3 pos_avatar = new Vector3(this.pos.x, 1f, this.pos.z);
        avatar.transform.position = pos_avatar;
        avatar.transform.rotation = this.rotation;
    }

    internal HashSet<GameObject> GetOwnedFigurines()
    {
        return this.ownedFigurines;
    }

    public void AddFigurine(GameObject fig)
    {
        this.ownedFigurines.Add(fig);
    }
}
