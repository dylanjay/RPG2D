using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Entity
{

    //private static PlayerEntity _instance = this;
    //public static PlayerEntity instance { get { return _instance; } }

    //Stats
    /*public int playerID = 0;
    public string playerName = "Dylan";
    public string type = "Player";
    public int playerHealth = 10;
    public int playerDefence = 5;
    public int playerAttack = 5;
    public int playerMagic = 5;
    public int playerMana = 5;*/

    //Level
    public int curLvl = 1;
    public const int maxLvl = 6;

    //Experience
    public int curExp = 0;
    public int[] expToLvl = new int[maxLvl] {0, 1, 2, 4, 8, 16 };

    public Player() : base()
    {

    }

    /*public Player(int id, Dictionary<string, int> stats) : base(id, stats)
    {

    }*/

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
                            attack += stat.value; //Change names accordingly
                            break;
                        case "Defence":
                            defence += stat.value;
                            break;
                        case "Vitality":
                            health += stat.value;
                            break;
                    }
                }
            }
        }
    }
}
