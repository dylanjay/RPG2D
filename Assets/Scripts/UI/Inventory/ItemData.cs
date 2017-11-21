using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ItemData : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public int stackAmount;
    public int slot;
    public int tab;

    public bool isEquipped = false;

    Inventory inventory;
    PlayerControl player;
    Tooltip tooltip;
    Vector2 offset;
    GameObject equipmentPanel;
    GameObject equipmentSlotPanel;
    GameObject canvas;
    GameObject inventoryPanel;

    bool dragged = false;

    void Start()
    {
        inventory = Inventory.instance;
        player = PlayerControl.instance;
        inventoryPanel = inventory.gameObject;
        equipmentPanel = inventoryPanel.transform.Find("Equipment Panel").gameObject;
        equipmentSlotPanel = equipmentPanel.transform.Find("Equipment Slot Panel").gameObject;
        tooltip = Canvas.instance.gameObject.GetComponent<Tooltip>();
    }

    public void setData(Item item, int stackAmount, int tab, int slot)
    {
        this.item = item;
        this.stackAmount = stackAmount;
        this.tab = tab;
        this.slot = slot;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null && eventData.button == PointerEventData.InputButton.Left)
        {
            offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
            transform.SetParent(Canvas.instance.gameObject.transform);
            transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            inventory.HoldItem();
        }

        if (item is Equippable)
        {
            Equippable equippable = item as Equippable;
            Inventory.EquipmentType equipmentSlot = equippable.equipmentType;

            //if slot is not equal to -1 the item is in the inventory so I am equipping
            if (item != null && stackAmount == 1 && eventData.button == PointerEventData.InputButton.Right && slot != -1)
            {
                if (inventory.equipmentSlots[equipmentSlot].id == -1)
                {
                    inventory.equipmentSlots[equipmentSlot] = item;
                    inventory.items[inventory.currentTab][slot] = new Item();
                    inventory.slots[inventory.currentTab][slot].name = "Empty Slot";
                }

                else
                {
                    Transform itemToReplace = equipmentSlotPanel.transform.Find(Inventory.EquipmentEnumToString(equipmentSlot)).Find(inventory.equipmentSlots[equipmentSlot].title);
                    ItemData itemToReplaceData = itemToReplace.GetComponent<ItemData>();
                    itemToReplaceData.tab = inventory.currentTab;
                    itemToReplaceData.slot = slot;
                    itemToReplaceData.isEquipped = false;
                    itemToReplace.transform.SetParent(inventory.slots[inventory.currentTab][slot].transform);
                    itemToReplace.transform.position = inventory.slots[inventory.currentTab][slot].transform.position;
                    inventory.slots[inventory.currentTab][slot].name = itemToReplace.GetComponent<ItemData>().item.title + " Slot";
                    inventory.equipmentSlots[equipmentSlot] = item;

                }
                slot = -1;
                Transform slotTransform = equipmentSlotPanel.transform.Find(Inventory.EquipmentEnumToString(equipmentSlot));
                transform.SetParent(slotTransform);
                transform.position = slotTransform.position;
                isEquipped = true;
                inventory.UpdateStats();
            }

            //else I am un-equipping
            else if (item != null && stackAmount == 1 && eventData.button == PointerEventData.InputButton.Right && slot == -1)
            {
                tooltip.Deactivate();
                inventory.AddItem(item.id);
                inventory.equipmentSlots[equipmentSlot] = new Item();
                Destroy(gameObject);
            }
        }
    }

    public void Reset()
    {

        if (isEquipped)
        {
            if (item is Equippable)
            {
                Inventory.EquipmentType equipmentType;
                Equippable equippable = item as Equippable;
                equipmentType = equippable.equipmentType;
                transform.SetParent(equipmentSlotPanel.transform.Find(Inventory.EquipmentEnumToString(equipmentType)));
                transform.position = equipmentSlotPanel.transform.Find(Inventory.EquipmentEnumToString(equipmentType)).position;
            }
        }
        else
        {
            transform.SetParent(inventory.slots[inventory.initialTab][slot].transform);
            transform.position = inventory.slots[inventory.initialTab][slot].transform.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!dragged) { Reset(); }

            else { dragged = false; }

            inventory.holdingItem = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragged = true;
        if (item != null)
        {
            transform.position = eventData.position - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if cursor is not hovering over the inventory drop item
        bool dropItem = true;
        
        foreach(GameObject hoveredObject in eventData.hovered)
        {
            if(hoveredObject == inventoryPanel || hoveredObject == equipmentPanel)
            {
                dropItem = false;
                break;
            }
        }

        if(dropItem)
        {
            inventory.items[inventory.initialTab][slot] = new Item();
            inventory.slots[inventory.initialTab][slot].name = "Empty Slot";
            player.DropItem(eventData.position, eventData.pointerDrag.GetComponent<ItemData>());
            Destroy(gameObject);
        }

        else
        {
            if (!inventory.hoveringOverSlot)
            {
                Reset();
            }
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();
    }
}
