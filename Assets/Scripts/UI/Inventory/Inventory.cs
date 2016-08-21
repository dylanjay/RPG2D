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
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    int slotSize;
    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();
    Player player;
    EntityDatabase entityDatabase;
    public Dictionary<string, Item> equipmentSlots = new Dictionary<string, Item>();

    public bool equippedItem = false;

    void Start()
    {
        _instance = this;

        database = GetComponent<ItemDatabase>();
        entityDatabase = EntityDatabase.instance;

        player = entityDatabase.GetEntityByID(0) as Player;

        player.UpdateStats(equipmentSlots);

        slotSize = 16;
        inventoryPanel = GameObject.Find("Inventory Panel");
        slotPanel = inventoryPanel.transform.FindChild("Slot Panel").gameObject;
        equipmentPanel = GameObject.Find("Equipment Slot Panel");
        for(int i = 0; i < slotSize; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().slotID = i;
            slots[i].transform.SetParent(slotPanel.transform);
            slots[i].name = "Empty Slot";
        }

        for(int i = 0; i < equipmentPanel.transform.childCount; i++)
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
            player.UpdateStats(equipmentSlots);
            equippedItem = false;
        }

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
