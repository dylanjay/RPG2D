﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ItemComponent : MonoBehaviour
{
    public Item item;

    public int itemID = -1;

    public int stackAmount = 1;

    void Start()
    {
        item = ItemDatabase.instance.GetItemByID(itemID);
        itemID = item.id;
        GetComponent<SpriteRenderer>().sprite = item.sprite;
    }

    //CR: UpperCamelCaseFunctionNames() 
    //CTRL + R, CTRL + R to rename.
    public void setStack(int num)
    {
        stackAmount = num;
    }

    //CR: UpperCamelCaseFunctionNames()
    public void reset(int id)
    {
        item = ItemDatabase.instance.GetItemByID(id);
        
        itemID = item.id;
        GetComponent<SpriteRenderer>().sprite = item.sprite;
    }
}
