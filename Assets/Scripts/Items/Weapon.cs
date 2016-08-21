using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : Item {

    public enum baseType { Sword, Spear, Axe }
    public baseType type;
    public string typeString;

	public Weapon() : base()
    {

    }

    public Weapon(int ID, string Title, string Type, int Value, string Desc, bool Stack, int Rarity, string Slug, List<Stat> Stats)
        : base(ID, Title, Value, Desc, Stack, Rarity, Slug, Stats)
    {
        typeString = Type;
        switch(Type)
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
