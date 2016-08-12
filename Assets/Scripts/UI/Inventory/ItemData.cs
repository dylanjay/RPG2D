using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ItemData : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

    public Item item;
    public int amount;
    public int slot;

    Inventory inv;
    Tooltip tooltip;
    Vector2 offset;

    bool dragged = false;

    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        tooltip = inv.GetComponent<Tooltip>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null)
        {
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            transform.SetParent(this.transform.parent.parent);
            transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!dragged)
        {
            transform.SetParent(inv.slots[slot].transform);
            transform.position = inv.slots[slot].transform.transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
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
