using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class EquipmentSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    Inventory.EquipmentType type;

    Inventory inventory;

    GameObject inventoryPanel;
    GameObject equipmentSlotPanel;

    void Start()
    {
        inventory = Inventory.instance;

        type = Inventory.EquipmentStringToEnum(name);

        inventoryPanel = inventory.transform.FindChild("Inventory Panel").gameObject;
        equipmentSlotPanel = inventory.transform.FindChild("Equipment Panel").FindChild("Equipment Slot Panel").gameObject;
    }

    void EquipItem(ItemData selectedItem, Inventory.EquipmentType equipmentSlot)
    {
        selectedItem.slot = -1;
        selectedItem.isEquipped = true;
        Transform slotTransform = equipmentSlotPanel.transform.FindChild(Inventory.EquipmentEnumToString(equipmentSlot));
        selectedItem.transform.SetParent(slotTransform);
        selectedItem.transform.position = slotTransform.position;
        inventory.equipmentSlots[equipmentSlot] = selectedItem.item;
        inventory.UpdateStats();
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemData selectedItem = eventData.pointerDrag.GetComponent<ItemData>();

        Equippable equippable = selectedItem.item as Equippable;
        Inventory.EquipmentType equipmentSlot = equippable.equipmentType;

        if (equipmentSlot != type)
        {
            selectedItem.Reset();
            return;
        }

        //equipment slot to equipment slot
        if (selectedItem.isEquipped)
        {
            //TODO if you can move among slots
        }

        //inventory slot to equipment slot - equipping
        else
        {
            //if this slot is empty fill it with item and empty item's previous slot
            if (inventory.equipmentSlots[equipmentSlot].id == -1)
            {
                inventory.items[inventory.initialTab][selectedItem.slot] = new Item();
                inventory.slots[inventory.initialTab][selectedItem.slot].name = "Empty Slot";

                EquipItem(selectedItem, equipmentSlot);
            }

            //If this slot is full swap items
            else
            {
                Transform itemToReplaceTransform = equipmentSlotPanel.transform.FindChild(Inventory.EquipmentEnumToString(equipmentSlot));
                ItemData itemToReplaceData = itemToReplaceTransform.GetComponent<ItemData>();
                if (itemToReplaceData.item is Equippable)
                {
                    Equippable itemToReplace = itemToReplaceTransform.GetComponent<ItemData>().item as Equippable;
                    Inventory.EquipmentType itemToReplaceType = itemToReplace.equipmentType;

                    if (itemToReplaceType == equipmentSlot)
                    {
                        itemToReplaceData.tab = inventory.initialTab;
                        itemToReplaceData.slot = selectedItem.slot;
                        itemToReplaceTransform.transform.SetParent(inventory.slots[inventory.initialTab][selectedItem.slot].transform);
                        itemToReplaceTransform.transform.position = inventory.slots[inventory.initialTab][selectedItem.slot].transform.position;
                        inventory.slots[inventory.initialTab][selectedItem.slot].name = itemToReplaceTransform.GetComponent<ItemData>().item.title + " Slot";

                        EquipItem(selectedItem, equipmentSlot);

                        inventory.items[inventory.initialTab][selectedItem.slot] = itemToReplaceTransform.GetComponent<ItemData>().item;
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Inventory.instance.hoveringOverSlot = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Inventory.instance.hoveringOverSlot = false;
    } 
}
