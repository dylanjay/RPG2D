using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ItemData : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

    public Item item;
    public int stackAmount;
    public int slot;
    public bool endInventory = true;
    public bool beginInventory = true;

    Inventory inv;
    PlayerControl player;
    Tooltip tooltip;
    Vector2 offset;
    GameObject equipmentPanel;
    //GameObject slotPanel;
    GameObject canvas;
    public string equipmentSlot;

    public bool clicked = false;
    public bool transferSuccess = false;

    bool dragged = false;

    void Start()
    {
        inv = Inventory.instance;
        player = PlayerControl.instance;
        tooltip = GameObject.Find("Inventory").GetComponent<Tooltip>();
        equipmentPanel = GameObject.Find("Equipment Slot Panel");
        //slotPanel = GameObject.Find("Slot Panel");
        canvas = GameObject.Find("Canvas");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clicked = true;
        if (item != null)
        {
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            transform.SetParent(canvas.transform);
            transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;

            if (eventData.hovered.Count > 0)
            {
                if (eventData.hovered[0] == equipmentPanel)
                {
                    beginInventory = false;
                }

                else
                {
                    beginInventory = true;
                }
            }
        }
    }

    public void Reset()
    {
        clicked = false;
        transform.SetParent(inv.slots[slot].transform);
        transform.position = inv.slots[slot].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if(!dragged)
        {
            Reset();
        }

        else
        {
            dragged = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragged = true;
        if (item != null)
        {
            transform.position = eventData.position - offset;
        }

        if (eventData.hovered.Count > 0)
        {
            if (eventData.hovered[0] == equipmentPanel)
            {
                endInventory = false;
            }

            else
            {
                endInventory = true;
            }
        }

        else
        {
            endInventory = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.hovered.Count > 0)
        {
            if (eventData.hovered[0] == equipmentPanel && transferSuccess)
            {
                Transform trans = equipmentPanel.transform.FindChild(equipmentSlot);
                transform.SetParent(trans);
                transform.position = trans.position;
                transferSuccess = false;
            }

            else
            {
                transform.SetParent(inv.slots[slot].transform);
                transform.position = inv.slots[slot].transform.transform.position;
            }
            clicked = false;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

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
            Destroy(this.gameObject);
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
