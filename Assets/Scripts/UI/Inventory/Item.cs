using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class Item {

    public int id { get; set; }
    public string title { get; set; }
    public int value { get; set; }
    /*public int power { get; set; }
    public int defence { get; set; }
    public int vitality { get; set; }*/
    public string description { get; set; }
    public bool stackable { get; set; }
    public int rarity { get; set; }
    public string slug { get; set; }

    public List<Stat> stats { get; set; }

    public Sprite sprite;

    /*

    public Item(int newID, string newTitle, int newValue, int newPower, int newDefence, int newVitality, string newDescription, bool newStackable, int newRarity, string newSlug)
    {
        id = newID;
        title = newTitle;
        value = newValue;
        power = newPower;
        defence = newDefence;
        vitality = newVitality;
        description = newDescription;
        stackable = newStackable;
        rarity = newRarity;
        slug = newSlug;
        sprite = Resources.Load<Sprite>("Items/" + slug);
    }*/

    public Item()
    {
        id = -1;
    }

    public void SetSprite()
    {
        sprite = Resources.Load<Sprite>("Items/" + slug);
    }
}
