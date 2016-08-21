using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class EntityDatabase : MonoBehaviour {

    public List<string> entityNames = new List<string>();
    public Dictionary<string, Entity> entityDict = new Dictionary<string, Entity>();
    JsonData data;

    private static EntityDatabase _instance;
    public static EntityDatabase instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
        data = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Entities.json"));
        FillDatabase();
    }

    public Entity GetEntityByID(int id)
    {
        string listName = entityNames[id];
        if (GetEntityByName(listName) == null)
        {
            return null;
        }
        int dictID = GetEntityByName(listName).id;
        if (id != dictID)
        {
            Debug.LogError(listName + " has the wrong id");
        }

        return GetEntityByName(listName);
    }

    public Entity GetEntityByName(string name)
    {
        if (entityDict.ContainsKey(name))
        {
            return entityDict[name];
        }
        return null;
    }

    void FillDatabase()
    {

        for (int i = 0; i < data.Count; i++)
        {
            string name = (string)data[i]["name"];
            entityNames.Add(name);
            Dictionary<string, int> stats = new Dictionary<string, int>();
            for (int j = 0; j < data[i]["stats"].Count; j++)
            {
                stats.Add((string)data[i]["stats"][j]["name"], (int)data[i]["stats"][j]["value"]);
            }

            if ((string)data[i]["type"] == "Player")
            {
                Player entity = new Player((int)data[i]["id"], stats);
                entityDict.Add(name, entity);
            }

            else if ((string)data[i]["type"] == "Hostile")
            {
                Hostile entity = new Hostile((int)data[i]["id"], stats, (int)data[i]["level"], (int)data[i]["exp"]);
                entityDict.Add(name, entity);
            }
            /*
            else if ((string)data[i]["type"] == "Neutral")
            {
                Neutral entity = new Neutral((int)data[i]["id"], (string)data[i]["title"], (int)data[i]["value"], (string)data[i]["description"], (bool)data[i]["stackable"],
                 (int)data[i]["rarity"], (string)data[i]["slug"], stats);
                entityDict.Add(name, entity);
            }*/

        }
    }

}
