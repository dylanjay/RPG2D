using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ItemData : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

    public Item item;
    public int amount;
    public int slot;

    Inventory inv;
    PlayerControl player;
    Tooltip tooltip;
    Vector2 offset;

    public bool clicked = false;

    bool dragged = false;

    void Start()
    {
        inv = Inventory.instance;
        player = GameObject.Find("Player").GetComponent<PlayerControl>();
        tooltip = GameObject.Find("Inventory").GetComponent<Tooltip>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clicked = true;
        if (item != null)
        {
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            transform.SetParent(this.transform.parent.parent);
            transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void Reset()
    {
        clicked = false;
        transform.SetParent(inv.slots[slot].transform);
        transform.position = inv.slots[slot].transform.transform.position;
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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        clicked = false;
        transform.SetParent(inv.slots[slot].transform);
        transform.position = inv.slots[slot].transform.transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
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
