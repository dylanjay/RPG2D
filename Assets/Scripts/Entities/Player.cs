using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Entity {

    //private static PlayerEntity _instance = this;
    //public static PlayerEntity instance { get { return _instance; } }

    int curLvl = 1;
    const int maxLvl = 69;

    int curExp = 0;
    int[] expToLvl = new int[maxLvl];

    void Awake()
    {
        exp
    }

    public Player() : base()
    {

    }

    public Player(int id, Dictionary<string, int> stats) : base(id, stats)
    {

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
