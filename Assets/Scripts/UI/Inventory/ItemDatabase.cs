using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDatabase : MonoBehaviour {

    public List<Item> database;
    JsonData itemData;

    private static ItemDatabase _instance;
    public static ItemDatabase instance { get { return _instance; } }

    //void Start()
    void Awake()
    {
        _instance = this;
        //itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
        Item[] items = JsonMapper.ToObject<Item[]>(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
        database = new List<Item>(items);
        SetSprites();
    }

    public Item FetchItemByID(int id)
    {
        for(int i = 0; i < database.Count; i++)
        {
            if (database[i].id == id)
            {
                return database[i];
            }
        }

        return null;
    }

    void SetSprites()
    {
        for(int i = 0; i < database.Count; i++)
        {
            database[i].SetSprite();
        }
    }
}
