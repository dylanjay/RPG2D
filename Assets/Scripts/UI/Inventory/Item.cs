﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class Item {

    public int id { get; set; }
    public string title { get; set; }
    public int value { get; set; }
    public string description { get; set; }
    public bool stackable { get; set; }
    public int rarity { get; set; }
    public string slug { get; set; }

    public List<Stat> stats { get; set; }

    public Sprite sprite;

    public Item()
    {
        id = -1;
    }

    public Item(int ID, string Title, int Value, string Desc, bool Stack, int Rarity, string Slug, List<Stat> Stats)
    {
        id = ID;
        title = Title;
        value = Value;
        description = Desc;
        stackable = Stack;
        rarity = Rarity;
        slug = Slug;
        stats = Stats;
        sprite = Resources.Load<Sprite>("Items/" + slug);
    }
}
