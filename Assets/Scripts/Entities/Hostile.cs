using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hostile : Entity {

    public int lvl { get; set; }
    public int expGiven { get; set; }
    Player player;

    public Hostile() : base()
    {

    }

    public Hostile(int id, Dictionary<string, int> stats, int lvl, int expGiven) : base(id, stats)
    {
        this.lvl = lvl;
        this.expGiven = expGiven;
    }

}
