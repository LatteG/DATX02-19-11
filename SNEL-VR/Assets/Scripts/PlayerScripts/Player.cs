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
    public bool avatarShowing;
    public Material playerColor;

    public void InitPlayer(Vector3 pos, Quaternion rotation, int ID, string name, Material color)
    {
        this.pos = pos;
        this.rotation = rotation;
        this.ID = ID;
        this.name = name;
        this.ownedFigurines = new HashSet<GameObject>();
        this.avatarShowing = false;
        this.playerColor = color;

        SetAvatar();
    }

    private void SetAvatar()
    {
        Vector3 pos_avatar = new Vector3(this.pos.x, 1f, this.pos.z);
        avatar = GameObject.Instantiate(GameObject.FindGameObjectWithTag("Avatar"), pos_avatar, this.rotation) as GameObject;

        //Better way to access children? Tag
        avatar.transform.GetChild(0).GetComponent<Renderer>().material = playerColor;
        avatar.transform.GetChild(1).GetComponent<Renderer>().material = playerColor;
        avatar.transform.GetChild(2).GetComponent<Renderer>().material = playerColor;
    }

    public Vector3 GetPlayerPos()
    {
        return this.pos;
    }

    public int GetPlayerID()
    {
        return this.ID;
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
        this.avatarShowing = false;
    }

    public void HideAvatar()
    {
        this.avatar.SetActive(false);
        this.avatarShowing = true;
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
