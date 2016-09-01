using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Entity {

    //private static PlayerEntity _instance = this;
    //public static PlayerEntity instance { get { return _instance; } }

    public int curLvl = 1;
    public const int maxLvl = 6;

    public int curExp = 0;
    public int[] expToLvl = new int[maxLvl] {0, 1, 2, 4, 8, 16 };

    public int combo = 0;

    public Player() : base()
    {

    }

    public Player(int id, Dictionary<string, int> stats) : base(id, stats)
    {

    }

    public void incrementCombo()
    {
        combo++;
        stats["Attack"]++;
        Debug.Log("Current combo multiplier: " + combo);
    }

    public void resetCombo()
    {
        stats["Attack"] -= combo;
        combo = 0;
        Debug.Log("Combo Reset");
    }

    public void setExp(int expAmount)
    {
        if (curLvl != maxLvl)
        {
            curExp += expAmount;
            for (int i = curLvl; i < maxLvl; i++)
            {
                if (curExp >= expToLvl[curLvl])
                {
                    curExp -= expToLvl[curLvl];
                    curLvl++; //function for lvling up?
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void UpdateStats(Dictionary<string, Item> equipmentSlots)
    {
        foreach(Item item in equipmentSlots.Values)
        {
            if (item.id != -1 && item != null)
            {
                for (int i = 0; i < item.stats.Count; i++)
                {
                    Stat stat = item.stats[i];

                    switch (stat.name)
                    {
                        case "Power":
                            stats["Attack"] += stat.value; //Change names accordingly
                            break;
                        case "Defence":
                            stats["Defence"] += stat.value;
                            break;
                        case "Vitality":
                            stats["Health"] += stat.value;
                            break;
                    }
                }
            }
        }
    }
}
