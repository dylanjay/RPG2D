using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Equippable : Item
{
    public Inventory.EquipmentType equipmentType;

    public Equippable() : base()
    {

    }

    public Equippable(int ID, string Title, int Value, string Desc, bool Stack, int Rarity, string Slug, Inventory.EquipmentType equipmentType, List<ItemStat> Stats)
        : base(ID, Title, Value, Desc, Stack, Rarity, Slug, Stats)
    {
        this.equipmentType = equipmentType;
    }
}
