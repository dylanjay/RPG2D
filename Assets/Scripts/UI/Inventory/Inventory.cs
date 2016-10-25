using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory instance { get; private set; }

    GameObject inventoryPanel;
    GameObject slotPanel;
    GameObject equipmentSlotPanel;
    ItemDatabase database;

    public GameObject inventorySlotPrefab;
    public GameObject inventoryItemPrefab;

    int slotSize = 16;
    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    public Dictionary<string, Item> equipmentSlots = new Dictionary<string, Item>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inventoryPanel = transform.FindChild("Inventory Panel").gameObject;
        slotPanel = inventoryPanel.transform.FindChild("Inventory Slot Panel").gameObject;
        equipmentSlotPanel = transform.FindChild("Equipment Panel").FindChild("Equipment Slot Panel").gameObject;

        database = ItemDatabase.instance;

        Player.instance.UpdateStats(equipmentSlots);

        //Populate slots and instantiate canvas objects
        for (int i = 0; i < slotSize; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlotPrefab));
            slots[i].GetComponent<InventorySlot>().slotID = i;
            slots[i].transform.SetParent(slotPanel.transform);
            slots[i].name = "Empty Slot";
        }

        for (int i = 0; i < equipmentSlotPanel.transform.childCount; i++)
        {
            equipmentSlots.Add(equipmentSlotPanel.transform.GetChild(i).name, new Item());
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

    public void UpdateStats()
    {
        Player.instance.UpdateStats(equipmentSlots);
    }

    GameObject CreateItemObject(Item itemToAdd, int stackAmount, int slot, Transform parent)
    {
        GameObject itemObj = Instantiate(inventoryItemPrefab);
        string equipmentSlot = "";
        if(itemToAdd is Weapon)
        {
            Weapon weapon = itemToAdd as Weapon;
            equipmentSlot = weapon.typeString;
        }
        else if(itemToAdd is Wearable)
        {
            Wearable wearable = itemToAdd as Wearable;
            equipmentSlot = wearable.typeString;
        }
        itemObj.GetComponent<ItemData>().setData(itemToAdd, stackAmount, slot, equipmentSlot);
        itemObj.transform.SetParent(parent);
        itemObj.transform.localPosition = Vector2.zero;
        itemObj.GetComponent<Image>().sprite = itemToAdd.sprite;
        itemObj.name = itemToAdd.title;
        return itemObj;
    }

    public void LoadInventory(List<SerializableItem> itemList, List<SerializableItem> equipmentList)
    {
        //Delete current inventory
        items.Clear();

        for (int i = 0; i < slotSize; i++)
        {
            //refill items with empty slots
            items.Add(new Item());
            slots[i].name = "Empty Slot";
            //destroy current slot
            if(slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }

        //Set new inventory
        foreach (SerializableItem item in itemList)
        {
            SetItem(item.id, item.stackAmount, item.slot);
        }

        //Delete current equipment
        equipmentSlots.Clear();
        for(int i = 0; i < equipmentSlotPanel.transform.childCount; i++)
        {
            //if slot has no child it is empty so don't destroy
            if(equipmentSlotPanel.transform.GetChild(i).childCount != 0)
            {
                Destroy(equipmentSlotPanel.transform.GetChild(i).GetChild(0).gameObject);
            }
        }

        //Set new equipment
        foreach (SerializableItem item in equipmentList)
        {
            //set equipment slot to new item and add to list
            if (item.id != -1)
            {
                equipmentSlots.Add(item.equipmentSlot, database.GetItemByID(item.id));
                CreateItemObject(database.GetItemByID(item.id), 1, -1, equipmentSlotPanel.transform.FindChild(item.equipmentSlot));
            }
            //it item id is -1 it is empty so add empty item
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
        GameObject itemObj = CreateItemObject(itemToAdd, stackAmount, slot, slots[slot].transform);
        if (stackAmount > 1)
        {
            itemObj.transform.FindChild("Stack Amount").GetComponent<Text>().text = stackAmount.ToString();
        }
        slots[slot].name = itemToAdd.title + " Slot";
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.GetItemByID(id);
        //if item already exists in inventory increment its stack count
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
        //else its a new item so create new item
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == -1)
                {
                    items[i] = itemToAdd;
                    CreateItemObject(itemToAdd, 1, i, slots[i].transform);
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
