using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Consumable : Item{

    public Consumable() : base()
    {

    }

    public Consumable(int ID, string Title, int Value, string Desc, bool Stack, int Rarity, string Slug, List<ItemStat> Stats)
        : base(ID, Title, Value, Desc, Stack, Rarity, Slug, Stats)
    {

    }
}
