﻿using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

public class ItemDatabase
{
    public static ItemDatabase instance { get; private set; }

    static public List<string> itemNames = new List<string>();
    static public Dictionary<string, Item> itemDict = new Dictionary<string, Item>();

    static public OrderedDictionary[] itemTierDicts = new OrderedDictionary[10];
    static JsonData data;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {        
        if(instance != null) { return; }

        instance = new ItemDatabase();
        data = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
        for(int i = 0; i < 10; i++)
        {
            itemTierDicts[i] = new OrderedDictionary();
        }
        FillDatabase();
    }

    public Item GetItemByID(int id)
    {
        string listName = itemNames[id];
        Item item = GetItemByName(listName);
        if (item == null)
        {
            return null;
        }
        int dictID = item.id;
        if (id != dictID)
        {
            Debug.LogError(listName + " has the wrong id");
        }

        return item;
    }

    public Item GetItemByName(string name)
    {
        for(int i = 0; i < 10; i++)
        {
            if(itemTierDicts[i].Contains(name))
            {
                return (Item)itemTierDicts[i][name];
            }
        }
        
        return null;
    }

    public Item GetRandomItem(int tier)
    {
        ICollection values = itemTierDicts[tier].Values;
        int size = itemTierDicts[tier].Count;
        Item[] items = new Item[size];
        values.CopyTo(items, 0);
        return items[Random.Range(0, size)];
    }

    static void FillDatabase()
    {
        for (int i = 0; i < data.Count; i++)
        {
            string name = (string)data[i]["title"];
            itemNames.Add(name);
            List<ItemStat> stats = new List<ItemStat>();
            for (int j = 0; j < data[i]["stats"].Count; j++)
            {
                stats.Add(new ItemStat((string)data[i]["stats"][j]["name"], (int)data[i]["stats"][j]["value"], (int)data[i]["stats"][j]["range"]));
            }

            int tier = (int)data[i]["tier"];

            if ((string)data[i]["type"] == "Weapon")
            {
                Inventory.EquipmentType equipmentType = Inventory.EquipmentStringToEnum((string)data[i]["equipmentType"]);
                Weapon item = new Weapon((int)data[i]["id"], (string)data[i]["title"], (string)data[i]["type"], (string)data[i]["subtype"], (int)data[i]["value"], (string)data[i]["description"], (bool)data[i]["stackable"],
                 tier, (string)data[i]["slug"], equipmentType, stats);
                itemTierDicts[tier].Add(name, item);
            }

            else if ((string)data[i]["type"] == "Wearable")
            {
                Inventory.EquipmentType equipmentType = Inventory.EquipmentStringToEnum((string)data[i]["equipmentType"]);
                Wearable item = new Wearable((int)data[i]["id"], (string)data[i]["title"], (string)data[i]["type"], (string)data[i]["subtype"], (int)data[i]["value"], (string)data[i]["description"], (bool)data[i]["stackable"],
                 tier, (string)data[i]["slug"], equipmentType, stats);
                itemTierDicts[tier].Add(name, item);
            }

            else if ((string)data[i]["type"] == "Consumable")
            {
                Consumable item = new Consumable((int)data[i]["id"], (string)data[i]["title"], (int)data[i]["value"], (string)data[i]["description"], (bool)data[i]["stackable"],
                 tier, (string)data[i]["slug"], stats);
                itemTierDicts[tier].Add(name, item);
            }
        }
    }
}
