using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ItemComponent : MonoBehaviour {

    public Item item;

    public int itemID = 0;
    public int stackAmount = 0;

    void Start()
    {
        item = ItemDatabase.instance.FetchItemByID(itemID);
        item.SetSprite();
        GetComponent<SpriteRenderer>().sprite = item.sprite;
    }

    public void setItem(int id, int stackNum)
    {
        itemID = id;
        stackAmount = stackNum;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        item.OnTriggerEnter2D(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        item.OnTriggerStay2D(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        item.OnTriggerExit2D(other);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        item.OnCollisionEnter2D(other);
    }

    void OnCollisionExit2D(Collision2D other)
    {
        item.OnCollisionEnter2D(other);
    }
}
