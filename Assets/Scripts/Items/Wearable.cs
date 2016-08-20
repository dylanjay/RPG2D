using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wearable : Item {

    public enum baseType { Chest, Boots, Head, Gloves, Jewelery }
    public baseType type;
    public string typeString;

    public Wearable() : base()
    {

    }

    public Wearable(int ID, string Title, string Type, int Value, string Desc, bool Stack, int Rarity, string Slug, List<Stat> Stats)
        : base(ID, Title, Value, Desc, Stack, Rarity, Slug, Stats)
    {
        typeString = Type;
        switch (Type)
        {
            case "Chest":
                type = baseType.Chest;
                break;

            case "Boots":
                type = baseType.Boots;
                break;

            case "Head":
                type = baseType.Head;
                break;

            case "Gloves":
                type = baseType.Gloves;
                break;

            case "Jewelery":
                type = baseType.Jewelery;
                break;
        }
    }
}
