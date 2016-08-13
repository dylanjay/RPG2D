using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class Item {

    public int id { get; set; }
    public string title { get; set; }
    public int value { get; set; }
    public string description { get; set; }
    public bool stackable { get; set; }
    public int rarity { get; set; }
    public string slug { get; set; }

    public List<Stat> stats { get; set; }

    public Sprite sprite;

    public Item()
    {
        id = -1;
    }

    public void SetSprite()
    {
        sprite = Resources.Load<Sprite>("Items/" + slug);
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {

    }

    public virtual void OnTriggerStay2D(Collider2D other)
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
