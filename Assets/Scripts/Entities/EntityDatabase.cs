using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class EntityDatabase : MonoBehaviour {

    public List<Entity> database;
    JsonData entityData;

    private static EntityDatabase _instance;
    public static EntityDatabase instance { get { return _instance; } }

    void Awake()
    {
        //entityData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Entities.json"));
        Entity[] entities = JsonMapper.ToObject<Entity[]>(File.ReadAllText(Application.dataPath + "/StreamingAssets/Entities.json"));
        database = new List<Entity>(entities);

        _instance = this;
        Debug.Log(database[0].stats[0].name);
    }

    public Entity FetchEntityByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].id == id)
            {
                return database[i];
            }
        }

        return null;
    }

}
