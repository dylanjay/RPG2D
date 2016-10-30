using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ItemData : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

    public Item item;
    public int stackAmount;
    public int slot;
    public int tab;
    public string equipmentSlot;

    Inventory inventory;
    PlayerControl player;
    Tooltip tooltip;
    Vector2 offset;
    GameObject equipmentSlotPanel;
    GameObject canvas;
    GameObject inventoryPanel;

    bool dragged = false;

    void Start()
    {
        inventory = Inventory.instance;
        player = PlayerControl.instance;
        canvas = GameObject.FindGameObjectWithTag("Screen Canvas");
        inventoryPanel = canvas.transform.FindChild("Inventory Panel").gameObject;
        equipmentSlotPanel = canvas.transform.FindChild("Equipment Panel").FindChild("Equipment Slot Panel").gameObject;
        tooltip = canvas.GetComponent<Tooltip>();
    }

    public void setData(Item item, int stackAmount, int tab, int slot, string equipmentSlot)
    {
        this.item = item;
        this.stackAmount = stackAmount;
        this.tab = tab;
        this.slot = slot;
        this.equipmentSlot = equipmentSlot;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null && eventData.button == PointerEventData.InputButton.Left)
        {
            offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
            transform.SetParent(canvas.transform);
            transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            inventory.HoldItem();
        }

        //if slot is not equal to -1 the item is in the inventory so I am equipping
        if (item != null && stackAmount == 1 && eventData.button == PointerEventData.InputButton.Right && slot != -1)
        {
            if (item is Weapon || item is Wearable)
            {
                if (inventory.equipmentSlots[equipmentSlot].id == -1)
                {
                    inventory.equipmentSlots[equipmentSlot] = item;
                    inventory.items[inventory.currentTab][slot] = new Item();
                    inventory.slots[inventory.currentTab][slot].name = "Empty Slot";
                }

                else
                {
                    Transform itemToReplace = equipmentSlotPanel.transform.FindChild(equipmentSlot).FindChild(inventory.equipmentSlots[equipmentSlot].title);
                    itemToReplace.GetComponent<ItemData>().tab = inventory.currentTab;
                    itemToReplace.GetComponent<ItemData>().slot = slot;
                    itemToReplace.transform.SetParent(inventory.slots[inventory.currentTab][slot].transform);
                    itemToReplace.transform.position = inventory.slots[inventory.currentTab][slot].transform.position;
                    inventory.slots[inventory.currentTab][slot].name = itemToReplace.GetComponent<ItemData>().item.title + " Slot";
                    inventory.equipmentSlots[equipmentSlot] = item;
                    
                }
                slot = -1;
                Transform slotTransform = equipmentSlotPanel.transform.FindChild(equipmentSlot);
                transform.SetParent(slotTransform);
                transform.position = slotTransform.position;
            }
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

    public void Reset()
    {
        transform.SetParent(inventory.slots[inventory.initialTab][slot].transform);
        transform.position = inventory.slots[inventory.initialTab][slot].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
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
            if(hoveredObject == inventoryPanel)
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
            bool hitSlot = false;
            foreach (GameObject hoveredObject in eventData.hovered)
            {
                if (hoveredObject.name.Substring(hoveredObject.name.Length - 4) == "Slot" && hoveredObject != inventory.slots[inventory.currentTab][slot])
                {
                    hitSlot = true;
                    break;
                }
            }

            if (!hitSlot)
            {
                Reset();
            }

            else
            {
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }

        /*else
        {
            bool hitSlot = false;
            Transform slotTrans = null;
            foreach (GameObject hoveredObject in eventData.hovered)
            {
                if (hoveredObject.name.Substring(hoveredObject.name.Length - 4) == "Slot" && hoveredObject != inventory.slots[inventory.currentTab][slot]) 
                {
                    hitSlot = true;
                    Debug.Log(hoveredObject.name);
                    slotTrans = hoveredObject.transform;
                    break;
                }
            }
            //if hit inventory slot transfer item
            if (hitSlot)
            {
                transform.SetParent(inventory.slots[inventory.currentTab][slot].transform);
                transform.position = inventory.slots[inventory.currentTab][slot].transform.transform.position;
            }

            //else reset item back
            else
            {
                transform.SetParent(inventory.slots[inventory.initialTab][slot].transform);
                transform.position = inventory.slots[inventory.initialTab][slot].transform.transform.position;
            }
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }*/

        /*if (eventData.hovered.Count > 0)
        {
            if ((eventData.hovered[0] == equipmentSlotPanel && (transferSuccess || (!transferSuccess && !beginInventory))) || eventData.hovered[0].name == "Slot Panel" && !transferSuccess && !beginInventory)
            {
                Transform trans = equipmentSlotPanel.transform.FindChild(equipmentSlot);
                transform.SetParent(trans);
                transform.position = trans.position;
                transferSuccess = false;
            }
            else
            {
                transform.SetParent(inv.slots[slot].transform);
                transform.position = inv.slots[slot].transform.transform.position;
            }
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        //drop item
        else
        {
            if (beginInventory)
            {
                inv.items[slot] = new Item();
                inv.slots[slot].name = "Empty Slot";
            }
            else
            {
                inv.equipmentSlots[equipmentSlot] = new Item();
            }
            player.DropItem(eventData.position, eventData.pointerDrag.GetComponent<ItemData>());
            Destroy(gameObject);
        }*/
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
