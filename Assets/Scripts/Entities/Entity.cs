using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : ScriptableObject{

    public int id { get; set; }
    //public string name { get; set; }
    public List<Stat> stats { get; set; }
    
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
