using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

    GameObject inventoryPanel;
    GameObject slotPanel;
    ItemDatabase database;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    int slotSize;
    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        database = GetComponent<ItemDatabase>();

        slotSize = 16;
        inventoryPanel = GameObject.Find("Inventory Panel");
        slotPanel = inventoryPanel.transform.FindChild("Slot Panel").gameObject;
        for(int i = 0; i < slotSize; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(slotPanel.transform);
            slots[i].name = "Empty Slot";
        }

        AddItem(0);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(1);
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemByID(id);
        if (itemToAdd.stackable && CheckInventory(itemToAdd))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == id)
                {
                    ItemData data = slots[i].transform.FindChild(itemToAdd.title).GetComponent<ItemData>();
                    data.amount++;
                    data.transform.FindChild("Stack Amount").GetComponent<Text>().text = data.amount.ToString();
                }
            }
        }

        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == -1)
                {
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem);
                    itemObj.GetComponent<ItemData>().item = itemToAdd;
                    itemObj.GetComponent<ItemData>().amount = 1;
                    itemObj.GetComponent<ItemData>().slot = i;
                    itemObj.transform.SetParent(slots[i].transform);
                    itemObj.transform.position = Vector2.zero;
                    itemObj.GetComponent<Image>().sprite = itemToAdd.sprite;
                    itemObj.name = itemToAdd.title;
                    slots[i].name = itemToAdd.title + " Slot";

                    break;
                }
            }
        }
    }

    bool CheckInventory(Item item)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if(items[i].id == item.id)
            {
                return true;
            }
        }
        return false;
    }
}
