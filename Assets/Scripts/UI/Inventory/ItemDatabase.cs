using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

public class ItemDatabase : MonoBehaviour {

    public List<string> itemNames = new List<string>();
    public Dictionary<string, Item> itemDict = new Dictionary<string, Item>();
    JsonData data;

    private static ItemDatabase _instance;
    public static ItemDatabase instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
        data = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
        FillDatabase();
    }



    public Item GetItemByID(int id)
    {
        string listName = itemNames[id];
        if(GetItemByName(listName) == null)
        {
            return null;
        }
        int dictID = GetItemByName(listName).id;
        if (id != dictID)
        {
            Debug.LogError(listName + " has the wrong id");
        }

        return GetItemByName(listName);
    }

    public Item GetItemByName(string name)
    {
        if(itemDict.ContainsKey(name))
        {
            return itemDict[name];
        }
        return null;
    }

    void FillDatabase()
    {
        for(int i = 0; i < data.Count; i++)
        {
            string name = (string)data[i]["title"];
            itemNames.Add(name);
            List<Stat> stats = new List<Stat>();
            for (int j = 0; j < data[i]["stats"].Count; j++)
            {
                stats.Add(new Stat((string)data[i]["stats"][j]["name"], (int)data[i]["stats"][j]["value"], (int)data[i]["stats"][j]["range"]));
            }

            if((string)data[i]["type"] == "Weapon")
            {
                Weapon item = new Weapon((int)data[i]["id"], (string)data[i]["title"], (string)data[i]["subtype"], (int)data[i]["value"], (string)data[i]["description"], (bool)data[i]["stackable"],
                 (int)data[i]["rarity"], (string)data[i]["slug"], stats);
                itemDict.Add(name, item);
            }

            else if((string)data[i]["type"] == "Wearable")
            {
                Wearable item = new Wearable((int)data[i]["id"], (string)data[i]["title"], (string)data[i]["subtype"], (int)data[i]["value"], (string)data[i]["description"], (bool)data[i]["stackable"],
                 (int)data[i]["rarity"], (string)data[i]["slug"], stats);
                itemDict.Add(name, item);
            }

            else if((string)data[i]["type"] == "Consumable")
            {
                Consumable item = new Consumable((int)data[i]["id"], (string)data[i]["title"], (int)data[i]["value"], (string)data[i]["description"], (bool)data[i]["stackable"],
                 (int)data[i]["rarity"], (string)data[i]["slug"], stats);
                itemDict.Add(name, item);
            }
        }
    }
}
