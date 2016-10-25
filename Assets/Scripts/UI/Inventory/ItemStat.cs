using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemStat{

    public string name { get; set; }

    public int value { get; set; }

    public int range { get; set; }

    public ItemStat()
    {
  
    }

    public ItemStat(string Name, int Value, int Range)
    {
        name = Name;
        value = Value;
        range = Range;
    }
}
