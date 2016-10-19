using UnityEngine;
using System.Collections;

public class HostilePack : MonoBehaviour {

    public float dropRate = 0.0f;

    int tier = 0;

    ItemDatabase itemDatabase;

    void DropItem(Item item)
    {
        GameObject itemInstance = Instantiate(Resources.Load("Prefabs/Item", typeof(GameObject)), transform.position, transform.rotation) as GameObject;
        itemInstance.GetComponent<ItemComponent>().reset(item.id);
        itemInstance.GetComponent<ItemComponent>().setStack(1);
    }

    void Start()
    {
        itemDatabase = ItemDatabase.instance;
    }

    void Update ()
    {
	    if(transform.childCount == 0)
        {
            if (Random.value <= dropRate)
            {
                DropItem(itemDatabase.GetRandomItem(tier));
            }
            Destroy(gameObject);
        }
	}
}
