using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory instance { get; private set; }

    GameObject inventoryPanel;
    GameObject tabPanel;
    GameObject slotPanel;
    GameObject equipmentSlotPanel;
    ItemDatabase database;

    public GameObject inventorySlotPrefab;
    public GameObject inventoryItemPrefab;
    public GameObject inventoryTabPrefab;

    public int numSlots;
    public int slotsPerTab;
    public int maxStackAmount;
    int numTabs;
    //public List<Item> items = new List<Item>();
    //public List<GameObject> slots = new List<GameObject>();
    public List<List<Item>> items = new List<List<Item>>();
    public List<List<GameObject>> slots = new List<List<GameObject>>();

    public Dictionary<string, Item> equipmentSlots = new Dictionary<string, Item>();

    public int currentTab = 0;

    public bool holdingItem = false;
    public int initialTab;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inventoryPanel = transform.FindChild("Inventory Panel").gameObject;
        tabPanel = inventoryPanel.transform.FindChild("Inventory Tab Panel").gameObject;
        slotPanel = inventoryPanel.transform.FindChild("Inventory Slot Panel").gameObject;
        equipmentSlotPanel = transform.FindChild("Equipment Panel").FindChild("Equipment Slot Panel").gameObject;

        database = ItemDatabase.instance;

        Player.instance.UpdateStats(equipmentSlots);

        numTabs = numSlots / slotsPerTab;
        if(numSlots % slotsPerTab != 0)
        {
            numTabs++;
        }

        //Populate slots and instantiate canvas objects
        for(int i = 0; i < numTabs; i++)
        {
            items.Add(new List<Item>());
            slots.Add(new List<GameObject>());
            GameObject newTab = Instantiate(inventoryTabPrefab);
            newTab.GetComponent<InventoryTab>().SetTabNumber(i);
            newTab.transform.SetParent(tabPanel.transform);

            int currentTabSlots = 0;
            if(numSlots % slotsPerTab != 0 && i == numTabs-1)
            {
                currentTabSlots = numSlots % slotsPerTab;
            }
            else
            {
                currentTabSlots = slotsPerTab;
            }
            for(int j = 0; j < currentTabSlots; j++)
            {
                items[i].Add(new Item());
                slots[i].Add(Instantiate(inventorySlotPrefab));
                slots[i][j].GetComponent<InventorySlot>().slotID = j;
                slots[i][j].transform.SetParent(slotPanel.transform);
                slots[i][j].name = "Empty Slot";

                if (i != 0)
                {
                    slots[i][j].SetActive(false);
                }
            }
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

    public void SwitchTabs(int tab)
    {
        for (int i = 0; i < slots[currentTab].Count; i++)
        {
            slots[currentTab][i].SetActive(false);
        }

        for(int i = 0; i < slots[tab].Count; i++)
        {
            slots[tab][i].SetActive(true);
        }
        currentTab = tab;
    }

    public void HoldItem()
    {
        holdingItem = true;
        initialTab = currentTab;
    }

    public void UpdateStats()
    {
        Player.instance.UpdateStats(equipmentSlots);
    }

    GameObject CreateItemObject(Item itemToAdd, int stackAmount, int tab, int slot, Transform parent)
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
        itemObj.GetComponent<ItemData>().setData(itemToAdd, stackAmount, tab, slot, equipmentSlot);
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

        for (int i = 0; i < numTabs; i++)
        {
            items.Add(new List<Item>());
            slots.Add(new List<GameObject>());

            int currentTabSlots = 0;
            if (numSlots % slotsPerTab != 0 && i == numTabs - 1)
            {
                currentTabSlots = numSlots % slotsPerTab;
            }
            else
            {
                currentTabSlots = slotsPerTab;
            }
            for (int j = 0; j < currentTabSlots; j++)
            {
                items[i].Add(new Item());
                slots[i][j].name = "Empty Slot";
                
                if(slots[i][j].transform.childCount > 0)
                {
                    //Destroy(slots[i][j].transform.FindChild(slots[i][j].name.Substring(0,slots[i][j].name.Length-5)).gameObject);
                    Destroy(slots[i][j].transform.GetChild(0).gameObject);
                }

                if (i != 0)
                {
                    slots[i][j].SetActive(false);
                }
            }
        }

        //Set new inventory
        foreach (SerializableItem item in itemList)
        {
            SetItem(item.id, item.stackAmount, item.tab, item.slot);
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
                CreateItemObject(database.GetItemByID(item.id), 1, -1, -1, equipmentSlotPanel.transform.FindChild(item.equipmentSlot));
            }
            //it item id is -1 it is empty so add empty item
            else
            {
                equipmentSlots.Add(item.equipmentSlot, new Item());
            }
        }
    }

    public void SetItem(int id, int stackAmount, int tab, int slot)
    {
        Item itemToAdd = database.GetItemByID(id);
        items[tab][slot] = itemToAdd;
        GameObject itemObj = CreateItemObject(itemToAdd, stackAmount, tab, slot, slots[tab][slot].transform);
        if (stackAmount > 1)
        {
            itemObj.transform.FindChild("Stack Amount").GetComponent<Text>().text = stackAmount.ToString();
        }
        slots[tab][slot].name = itemToAdd.title + " Slot";
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.GetItemByID(id);
        //if item already exists in inventory increment its stack count
        if (itemToAdd.stackable && CheckInventory(itemToAdd))
        {
            for (int i = 0; i < items.Count; i++)
            {
                for (int j = 0; j < items[i].Count; j++)
                {
                    if (items[i][j].id == id)
                    {
                        ItemData data = slots[i][j].transform.FindChild(itemToAdd.title).GetComponent<ItemData>();
                        data.stackAmount++;
                        data.transform.FindChild("Stack Amount").GetComponent<Text>().text = data.stackAmount.ToString();
                        return;
                    }
                }
            }
        }
        //else its a new item so create new item
        else
        {
            if(currentTab != 0)
            {
                bool checkCurrent = false;
                for (int i = currentTab; i < items.Count; i++)
                {
                    if (i == currentTab && checkCurrent)
                    {
                        continue;
                    }
                    for (int j = 0; j < items[i].Count; j++)
                    {
                        items[i][j] = itemToAdd;
                        CreateItemObject(itemToAdd, 1, i, j, slots[i][j].transform);
                        slots[i][j].name = itemToAdd.title + " Slot";
                        return;
                    }
                    if (i == currentTab && !checkCurrent)
                    {
                        i = 0;
                        checkCurrent = true;
                    }
                }
            }

            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    for (int j = 0; j < items[i].Count; j++)
                    {
                        if (items[i][j].id == -1)
                        {
                            items[i][j] = itemToAdd;
                            CreateItemObject(itemToAdd, 1, i, j, slots[i][j].transform);
                            slots[i][j].name = itemToAdd.title + " Slot";
                            return;
                        }
                    }
                }
            }

            /*
            for (int i = 0; i < items.Count; i++)
            {
                for (int j = 0; j < items[i].Count; j++)
                {
                    if (items[i][j].id == -1)
                    {
                        items[i][j] = itemToAdd;
                        CreateItemObject(itemToAdd, 1, j, slots[i][j].transform);
                        slots[i][j].name = itemToAdd.title + " Slot";
                        return;
                    }
                }
            }*/
        }
    }

    bool CheckInventory(Item item)
    {
        for(int i = 0; i < items.Count; i++)
        {
            for (int j = 0; j < items[i].Count; j++)
            {
                if (items[i][j].id == item.id)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
