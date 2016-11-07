﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : Equippable
{
    public enum baseType { Sword, Spear, Axe }
    public baseType type;
    public string typeString;

	public Weapon() : base()
    {

    }

    public Weapon(int ID, string Title, string Type, string SubType, int Value, string Desc, bool Stack, int Rarity, string Slug, Inventory.EquipmentType EquipmentType, List<ItemStat> Stats)
        : base(ID, Title, Value, Desc, Stack, Rarity, Slug, EquipmentType, Stats)
    {
        typeString = Type;
        switch(SubType)
        {
            case "Sword":
                type = baseType.Sword;
                break;

            case "Spear":
                type = baseType.Spear;
                break;

            case "Axe":
                type = baseType.Axe;
                break;
        }
    }
}
