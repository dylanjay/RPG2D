using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wearable : Equippable
{
    public enum baseType { Chest, Boots, Head, Gloves, Jewelery }
    public baseType type;
    public string typeString;

    public Wearable() : base()
    {

    }

    public Wearable(int ID, string Title, string Type, string SubType, int Value, string Desc, bool Stack, int Rarity, string Slug, Inventory.EquipmentType EquipmentType, List<ItemStat> Stats)
        : base(ID, Title, Value, Desc, Stack, Rarity, Slug, EquipmentType, Stats)
    {
        typeString = SubType;
        switch (SubType)
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
