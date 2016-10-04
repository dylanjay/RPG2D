using UnityEngine;
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
    public int tier { get; set; }
    public string slug { get; set; }

    public List<Stat> stats { get; set; }

    public Sprite sprite;

    public Item()
    {
        id = -1;
    }

    public Item(int id, string title, int value, string desc, bool stack, int tier, string slug, List<Stat> stats)
    {
        this.id = id;
        this.title = title;
        this.value = value;
        description = desc;
        stackable = stack;
        this.tier = tier;
        this.slug = slug;
        this.stats = stats;
        sprite = Resources.Load<Sprite>("Items/" + slug);
    }
}
