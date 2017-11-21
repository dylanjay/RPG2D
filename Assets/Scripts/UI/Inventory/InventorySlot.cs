using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int slotID;

    Inventory inventory;

    GameObject inventoryPanel;
    GameObject equipmentSlotPanel;

    void Start()
    {
        inventory = Inventory.instance;

        inventoryPanel = inventory.transform.Find("Inventory Panel").gameObject;
        equipmentSlotPanel = inventory.transform.Find("Equipment Panel").Find("Equipment Slot Panel").gameObject;
    }

    void PlaceItem(ItemData selectedItem)
    {
        selectedItem.tab = inventory.currentTab;
        selectedItem.slot = slotID;
        selectedItem.transform.SetParent(transform);
        selectedItem.transform.position = transform.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemData selectedItem = eventData.pointerDrag.GetComponent<ItemData>();

        if (selectedItem.slot == slotID)
        {
            selectedItem.Reset();
            return;
        }

        //equipment slot to inventory slot - unequipping
        if (selectedItem.isEquipped)
        {
            Equippable equippable = selectedItem.item as Equippable;
            Inventory.EquipmentType equipmentSlot = equippable.equipmentType;

            //if this slot is empty fill it with item and empty item's previous slot
            if (inventory.items[inventory.currentTab][slotID].id == -1)
            {
                selectedItem.isEquipped = false;
                inventory.equipmentSlots[equipmentSlot] = new Item();
                PlaceItem(selectedItem);
                inventory.items[inventory.currentTab][slotID] = selectedItem.item;
                inventory.UpdateStats();
            }

            //If this slot is full and item can be equipped - swap items
            else if (selectedItem.slot != slotID)
            {
                Transform itemToReplaceTransform = transform.Find(transform.name.Substring(0, transform.name.Length - 5));
                ItemData itemToReplaceData = itemToReplaceTransform.GetComponent<ItemData>();
                if (itemToReplaceData.item is Equippable)
                {
                    Equippable itemToReplace = itemToReplaceTransform.GetComponent<ItemData>().item as Equippable;
                    Inventory.EquipmentType itemToReplaceType = itemToReplace.equipmentType;

                    if(itemToReplaceType == equipmentSlot)
                    {
                        itemToReplaceData.isEquipped = true;
                        selectedItem.isEquipped = false;
                        itemToReplaceData.slot = -1;
                        Transform slotTransform = equipmentSlotPanel.transform.Find(Inventory.EquipmentEnumToString(equipmentSlot));
                        itemToReplaceTransform.SetParent(slotTransform);
                        itemToReplaceTransform.position = slotTransform.position;
                        PlaceItem(selectedItem);

                        inventory.equipmentSlots[equipmentSlot] = itemToReplaceData.item;
                        inventory.items[inventory.currentTab][slotID] = selectedItem.item;
                        inventory.slots[inventory.currentTab][slotID].name = selectedItem.item.title + " Slot";
                        inventory.UpdateStats();
                    }
                }
            }
        }

        //inventory slot to inventory slot
        else
        {
            //if this slot is empty fill it with item and empty item's previous slot
            if (inventory.items[inventory.currentTab][slotID].id == -1)
            {
                inventory.items[inventory.initialTab][selectedItem.slot] = new Item();
                inventory.slots[inventory.initialTab][selectedItem.slot].name = "Empty Slot";
                inventory.items[inventory.currentTab][slotID] = selectedItem.item;
                inventory.slots[inventory.currentTab][slotID].name = selectedItem.item.title + " Slot";
                PlaceItem(selectedItem);
            }

            //If this slot is full swap items
            else if (selectedItem.slot != slotID)
            {
                Transform itemToReplace = transform.Find(transform.name.Substring(0, transform.name.Length - 5));
                itemToReplace.GetComponent<ItemData>().tab = inventory.initialTab;
                itemToReplace.GetComponent<ItemData>().slot = selectedItem.slot;
                itemToReplace.transform.SetParent(inventory.slots[inventory.initialTab][selectedItem.slot].transform);
                itemToReplace.transform.position = inventory.slots[inventory.initialTab][selectedItem.slot].transform.position;
                inventory.slots[inventory.initialTab][selectedItem.slot].name = itemToReplace.GetComponent<ItemData>().item.title + " Slot";

                PlaceItem(selectedItem);

                inventory.items[inventory.initialTab][selectedItem.slot] = itemToReplace.GetComponent<ItemData>().item;
                inventory.items[inventory.currentTab][slotID] = selectedItem.item;
                inventory.slots[inventory.currentTab][slotID].name = selectedItem.item.title + " Slot";
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventory.hoveringOverSlot = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventory.hoveringOverSlot = false;
    }
}
