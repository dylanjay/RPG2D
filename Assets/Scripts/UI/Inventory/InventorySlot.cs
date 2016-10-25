using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotID;

    Inventory inventory;

    GameObject equipmentPanel;
    GameObject inventoryPanel;

    void Start()
    {
        inventory = Inventory.instance;
        equipmentPanel = transform.parent.parent.parent.FindChild("Equipment Panel").gameObject;
        inventoryPanel = transform.parent.parent.parent.FindChild("Inventory Panel").gameObject;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemData selectedItem = eventData.pointerDrag.GetComponent<ItemData>();

        //if this slot is empty fill it with item and empty item's previous slot
        if (inventory.items[slotID].id == -1)
        {
            inventory.items[selectedItem.slot] = new Item();
            inventory.slots[selectedItem.slot].name = "Empty Slot";
            inventory.items[slotID] = selectedItem.item;
            inventory.slots[slotID].name = selectedItem.item.title + " Slot";
            selectedItem.slot = slotID;
        }

        //If this slot is full swap items
        else if (selectedItem.slot != slotID)
        {
            Transform item = transform.FindChild(transform.name.Substring(0, transform.name.Length-5));
            item.GetComponent<ItemData>().slot = selectedItem.slot;
            item.transform.SetParent(inventory.slots[selectedItem.slot].transform);
            item.transform.position = inventory.slots[selectedItem.slot].transform.position;
            inventory.slots[selectedItem.slot].name = item.GetComponent<ItemData>().item.title + " Slot";

            selectedItem.slot = slotID;
            selectedItem.transform.SetParent(transform);
            selectedItem.transform.position = transform.position;

            inventory.items[selectedItem.slot] = item.GetComponent<ItemData>().item;
            inventory.items[slotID] = selectedItem.item;
            inventory.slots[slotID].name = selectedItem.item.title + " Slot";
        }
    }

    /*public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        bool endInventory = true;
        if (eventData.hovered.Count > 0)
        {
            if (eventData.hovered[1] == equipmentPanel)
            {
                eventData.hovered.Remove(eventData.hovered[0]);
            }

            if (eventData.hovered[0] == equipmentPanel || eventData.hovered[1] == equipmentPanel)
            {
                endInventory = false;
            }
        }

        if (droppedItem.beginInventory && endInventory)
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

        else if (droppedItem.beginInventory && !endInventory)
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

        else if (!droppedItem.beginInventory && endInventory)
        {
            droppedItem.transferSuccess = true;
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

                item.transform.SetParent(equipmentPanel.transform.FindChild(itemData.equipmentSlot));
                item.transform.position = equipmentPanel.transform.FindChild(itemData.equipmentSlot).position;

                droppedItem.slot = slotID;
                droppedItem.transform.SetParent(this.transform);
                droppedItem.transform.position = this.transform.position;

                inv.items[slotID] = droppedItem.item;
                inv.slots[slotID].name = droppedItem.item.title + " Slot";
            }
        }
    }*/
}
