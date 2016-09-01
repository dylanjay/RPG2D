using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hostile : Entity {

    //Stats
    /*public int hostileID = 1;
    public string hostileName = "Mark";
    public string type = "Hostile";
    public int hostileHealth = 10;
    public int hostileDefence = 5;
    public int hostileAttack = 5;
    public int hostileMagic = 5;
    public int hostileMana = 5;*/

    public int lvl = 1;
    public int expGiven = 1;
    Player player;

    public Hostile() : base()
    {

    }

    /*public Hostile(int id, Dictionary<string, int> stats, int lvl, int expGiven) : base(id, stats)
    {
        this.lvl = lvl;
        this.expGiven = expGiven;
    }*/

}
