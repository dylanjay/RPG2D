using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity{

    public int id { get; set; }
    //public string name { get; set; }
    public Dictionary<string, int> stats { get; set; }

    public Entity()
    {
        id = -1;
    }

    public Entity(int id, Dictionary<string, int> stats)
    {
        this.id = id;
        this.stats = stats;
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {

    }

    public virtual void OnCollisionEnter2D(Collision2D other)
    {

    }

    public virtual void OnCollisionExit2D(Collision2D other)
    {

    }

}
