using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

    public static Inventory instance { get { return _instance; } }
    private static Inventory _instance;

    GameObject inventoryPanel;
    GameObject slotPanel;
    GameObject equipmentPanel;
    ItemDatabase database;

    //CR: Prefabs should have names that end in Prefab.
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    int slotSize = 16;
    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    public Dictionary<string, Item> equipmentSlots = new Dictionary<string, Item>();

    public bool equippedItem = false;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        inventoryPanel = GameObject.Find("Inventory Panel");
        slotPanel = inventoryPanel.transform.FindChild("Slot Panel").gameObject;
        equipmentPanel = GameObject.Find("Equipment Slot Panel");

        database = GetComponent<ItemDatabase>();


        Player.instance.UpdateStats(equipmentSlots);

        for (int i = 0; i < slotSize; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().slotID = i;
            slots[i].transform.SetParent(slotPanel.transform);
            slots[i].name = "Empty Slot";
        }

        for (int i = 0; i < equipmentPanel.transform.childCount; i++)
        {
            equipmentSlots.Add(equipmentPanel.transform.GetChild(i).name, new Item());
        }

        //NEEDS TO BE DELETED
        AddItem(0);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(1);
    }

    void Update() {
        
        if (equippedItem)
        {
            Player.instance.UpdateStats(equipmentSlots);
            equippedItem = false;
        }

    }

    public void LoadInventory(List<SerializableItem> itemList, List<SerializableItem> equipmentList)
    {
        items.Clear();

        for (int i = 0; i < slotSize; i++)
        {
            items.Add(new Item());
            slots[i].name = "Empty Slot";
            if(slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }

        foreach (SerializableItem item in itemList)
        {
            SetItem(item.id, item.stackAmount, item.slot);
        }

        equipmentSlots.Clear();
        for(int i = 0; i < equipmentPanel.transform.childCount; i++)
        {
            if(equipmentPanel.transform.GetChild(i).childCount != 0)
            {
                Destroy(equipmentPanel.transform.GetChild(i).GetChild(0).gameObject);
            }
        }

        foreach (SerializableItem item in equipmentList)
        {
            if (item.id != -1)
            {
                equipmentSlots.Add(item.equipmentSlot, database.GetItemByID(item.id));
                Item itemToAdd = database.GetItemByID(item.id);
                GameObject itemObj = Instantiate(inventoryItem);
                itemObj.GetComponent<ItemData>().item = itemToAdd;
                itemObj.GetComponent<ItemData>().stackAmount = 1;
                itemObj.transform.SetParent(equipmentPanel.transform.FindChild(item.equipmentSlot));
                itemObj.transform.localPosition = Vector2.zero;
                itemObj.GetComponent<Image>().sprite = itemToAdd.sprite;
            }
            else
            {
                equipmentSlots.Add(item.equipmentSlot, new Item());
            }
        }
    }

    public void SetItem(int id, int stackAmount, int slot)
    {
        Item itemToAdd = database.GetItemByID(id);
        items[slot] = itemToAdd;
        GameObject itemObj = Instantiate(inventoryItem);
        itemObj.GetComponent<ItemData>().item = itemToAdd;
        itemObj.GetComponent<ItemData>().stackAmount = stackAmount;
        itemObj.GetComponent<ItemData>().slot = slot;
        if (stackAmount > 1)
        {
            itemObj.transform.FindChild("Stack Amount").GetComponent<Text>().text = stackAmount.ToString();
        }
        itemObj.transform.SetParent(slots[slot].transform);
        itemObj.transform.localPosition = Vector2.zero;
        itemObj.GetComponent<Image>().sprite = itemToAdd.sprite;
        itemObj.name = itemToAdd.title;
        slots[slot].name = itemToAdd.title + " Slot";
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.GetItemByID(id);
        if (itemToAdd.stackable && CheckInventory(itemToAdd))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == id)
                {
                    ItemData data = slots[i].transform.FindChild(itemToAdd.title).GetComponent<ItemData>();
                    data.stackAmount++;
                    data.transform.FindChild("Stack Amount").GetComponent<Text>().text = data.stackAmount.ToString();
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
                    itemObj.GetComponent<ItemData>().stackAmount = 1;
                    itemObj.GetComponent<ItemData>().slot = i;
                    itemObj.transform.SetParent(slots[i].transform);
                    itemObj.transform.localPosition = Vector2.zero;
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
