using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stat{

    public string name { get; set; }

    public int value { get; set; }

    public int range { get; set; }

    public Stat()
    {
  
    }

    public Stat(string Name, int Value, int Range)
    {
        name = Name;
        value = Value;
        range = Range;
    }
}
