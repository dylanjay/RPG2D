using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDatabase : MonoBehaviour {

    public List<Item> database;
    JsonData itemData;

    void Start()
    {
        //itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
        Item[] items = JsonMapper.ToObject<Item[]>(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
        database = new List<Item>(items);
        Debug.Log(database[0].stats.Count);
        setSprites();
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

    void setSprites()
    {
        for(int i = 0; i < database.Count; i++)
        {
            database[i].setSprite();
        }
    }
}
