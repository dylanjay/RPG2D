using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour, IDropHandler {

    public int id;

    private Inventory inv;

    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        if(inv.items[id].ID == -1)
        {
            inv.items[droppedItem.slot] = new Item();
            inv.slots[droppedItem.slot].name = "Empty Slot";
            inv.items[id] = droppedItem.item;
            inv.slots[id].name = droppedItem.item.Title + " Slot";
            droppedItem.slot = id;
        }

        else if(droppedItem.slot != id)
        {
            Transform item = this.transform.GetChild(0);
            item.GetComponent<ItemData>().slot = droppedItem.slot;
            item.transform.SetParent(inv.slots[droppedItem.slot].transform);
            item.transform.position = inv.slots[droppedItem.slot].transform.position;
            inv.slots[droppedItem.slot].name = item.GetComponent<ItemData>().item.Title + " Slot";

            droppedItem.slot = id;
            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;

            inv.items[droppedItem.slot] = item.GetComponent<ItemData>().item;
            inv.items[id] = droppedItem.item;
            inv.slots[id].name = droppedItem.item.Title + " Slot";
        }
    }
}
