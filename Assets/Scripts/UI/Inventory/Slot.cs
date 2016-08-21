using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour, IDropHandler {

    public int slotID;

    Inventory inv;
    Dictionary<string, Item> equipment;
    Transform equipmentPanel;

    void Start()
    {
        inv = Inventory.instance;
        equipment = inv.equipmentSlots;
        equipmentPanel = GameObject.Find("Equipment Slot Panel").transform;
    }

    public void OnDrop(PointerEventData eventData)
    {
        
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();

        if (droppedItem.beginInventory && droppedItem.endInventory)
        {
            if (inv.items[slotID].id == -1)
            {
                inv.items[droppedItem.slot] = new Item();
                inv.slots[droppedItem.slot].name = "Empty Slot";
                inv.items[slotID] = droppedItem.item;
                inv.slots[slotID].name = droppedItem.item.title + " Slot";
                droppedItem.slot = slotID;
            }

            else if (droppedItem.slot != slotID)
            {
                Transform item = this.transform.GetChild(0);
                item.GetComponent<ItemData>().slot = droppedItem.slot;
                item.transform.SetParent(inv.slots[droppedItem.slot].transform);
                item.transform.position = inv.slots[droppedItem.slot].transform.position;
                inv.slots[droppedItem.slot].name = item.GetComponent<ItemData>().item.title + " Slot";

                droppedItem.slot = slotID;
                droppedItem.transform.SetParent(this.transform);
                droppedItem.transform.position = this.transform.position;

                inv.items[droppedItem.slot] = item.GetComponent<ItemData>().item;
                inv.items[slotID] = droppedItem.item;
                inv.slots[slotID].name = droppedItem.item.title + " Slot";
            }
        }

        else if (droppedItem.beginInventory && !droppedItem.endInventory)
        {
            Item curItem = droppedItem.item;
            if ((curItem is Weapon && this.name == "Weapon") || (curItem is Wearable))
            {
                bool check = true;
                if(curItem is Wearable)
                {
                    Wearable itemWearable = curItem as Wearable;
                    if(itemWearable.typeString != this.name)
                    {
                        check = false;
                    }
                }

                if (check)
                {
                    inv.equippedItem = true;
                    droppedItem.transferSuccess = true;
                    if (equipment[this.name].id == -1)
                    {
                        equipment[this.name] = droppedItem.item;
                        inv.items[droppedItem.slot] = new Item();
                        inv.slots[droppedItem.slot].name = "Empty Slot";
                        droppedItem.slot = -1;
                        droppedItem.equipmentSlot = this.name;
                    }

                    else
                    {
                        Transform item = this.transform.GetChild(0);
                        item.GetComponent<ItemData>().slot = droppedItem.slot;
                        item.transform.SetParent(inv.slots[droppedItem.slot].transform);
                        item.transform.position = inv.slots[droppedItem.slot].transform.position;
                        inv.slots[droppedItem.slot].name = item.GetComponent<ItemData>().item.title + " Slot";

                        equipment[this.name] = droppedItem.item;
                        droppedItem.slot = -1;
                        droppedItem.equipmentSlot = this.name;
                    }
                }
            }
        }

        else if (!droppedItem.beginInventory && droppedItem.endInventory)
        {
            if (inv.items[slotID].id == -1)
            {
                equipment[droppedItem.equipmentSlot] = new Item();
                inv.items[slotID] = droppedItem.item;
                inv.slots[slotID].name = droppedItem.item.title + " Slot";
                droppedItem.slot = slotID;
            }

            else
            {
                Transform item = transform.GetChild(0);
                ItemData itemData = item.GetComponent<ItemData>();
                equipment[droppedItem.equipmentSlot] = itemData.item;
                itemData.slot = -1;
                itemData.equipmentSlot = droppedItem.equipmentSlot;
                droppedItem.equipmentSlot = "";

                item.transform.SetParent(equipmentPanel.FindChild(itemData.equipmentSlot));
                item.transform.position = equipmentPanel.FindChild(itemData.equipmentSlot).position;

                droppedItem.slot = slotID;
                droppedItem.transform.SetParent(this.transform);
                droppedItem.transform.position = this.transform.position;

                inv.items[slotID] = droppedItem.item;
                inv.slots[slotID].name = droppedItem.item.title + " Slot";
            }
        }
    }
}
