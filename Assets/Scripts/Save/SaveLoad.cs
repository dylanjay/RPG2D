using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/*public abstract class Serializable
{
    public abstract void Fill();
    public abstract void Load();
}*/

[System.Serializable]
public class SerializableVector2
{
    public float x;
    public float y;

    public void Fill(Vector2 v2)
    {
        x = v2.x;
        y = v2.y;
    }

    public Vector2 vector2 { get { return new Vector2(x, y); } }
}

[System.Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public void Fill(Vector3 v3)
    {
        x = v3.x;
        y = v3.y;
        z = v3.z;
    }

    public void Load(GameObject target)
    {
        target.transform.position = new Vector3(x, y, z);
    }

    public Vector3 vector3 { get { return new Vector3(x, y, z); } }
}

[System.Serializable]
public class SerializableQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public void Fill(Quaternion q)
    {
        x = q.x;
        y = q.y;
        z = q.z;
        w = q.w;
    }

    public void Load(GameObject target)
    {
        target.transform.rotation = new Quaternion(x, y, z, w);
    }

    public Quaternion quaternion { get { return new Quaternion(x, y, z, w); } }
}

[System.Serializable]
public class SerializableAbilities
{
    List<string> abilityList = new List<string>();
    List<string> slots = new List<string>();
    List<string> slotSprites = new List<string>();

    public void Fill(List<Ability> abilities)
    {
        foreach (Ability ability in abilities)
        {
            abilityList.Add(ability.name);
        }
        slots = SkillTree.instance.slots;
        slotSprites = SkillTree.instance.slotSprites;
    }

    public void Load(GameObject player)
    {
        AbilityManager.instance.EquipAbilities(abilityList, player);
        SkillTree.instance.LoadSlots(slots, slotSprites);
    }
}

[System.Serializable]
public class SerializableItem
{
    public int id;
    public int stackAmount;
    public int slot;
    public string equipmentSlot;

    public void Fill(ItemData data)
    {
        id = data.item.id;
        stackAmount = data.stackAmount;
        slot = data.slot;
    }

    public void Fill(int id, string equipmentSlot)
    {
        this.id = id;
        this.equipmentSlot = equipmentSlot;
    }
}

[System.Serializable]
public class SerializableInventory
{
    List<SerializableItem> items = new List<SerializableItem>();
    List<SerializableItem> equipment = new List<SerializableItem>();

    public void Fill(List<GameObject> itemList, Dictionary<string, Item> equipmentList)
    {
        foreach(GameObject item in itemList)
        {
            if (item.transform.childCount > 0)
            {
                SerializableItem serItem = new SerializableItem();
                serItem.Fill(item.transform.GetChild(0).GetComponent<ItemData>());
                items.Add(serItem);
            }
        }

        foreach(KeyValuePair<string,Item> item in equipmentList)
        {
            SerializableItem serItem = new SerializableItem();
            serItem.Fill(item.Value.id, item.Key);
            equipment.Add(serItem);
        }
    }

    public void Load()
    {
        Inventory.instance.LoadInventory(items, equipment);
    }
}

[System.Serializable]
public class GameState
{
    public static GameState current;

    public SerializableVector3 playerPosition = new SerializableVector3();
    public SerializableVector2 playerDirection = new SerializableVector2();
    public SerializableAbilities playerAbilities = new SerializableAbilities();
    public SerializableInventory playerInventory = new SerializableInventory();

    public GameState()
    {
        GameObject player = GameObject.Find("Player");

        playerPosition.Fill(player.transform.position);
        playerDirection.Fill(PlayerControl.instance.lastDirection);
        playerAbilities.Fill(AbilityManager.instance.equippedAbilities);
        playerInventory.Fill(Inventory.instance.slots, Inventory.instance.equipmentSlots);
    }

    public void Load()
    {
        GameObject player = GameObject.Find("Player");
        playerPosition.Load(player);
        PlayerControl.instance.SetDirection(playerDirection.vector2);
        playerAbilities.Load(player);
        playerInventory.Load();
    }
}

public static class SaveLoad
{
    public static List<GameState> savedGames = new List<GameState>();

    public static void Save()
    {
        if(savedGames.Count == 0)
        {
            savedGames.Add(new GameState());
        }
        else
        {
            savedGames[0] = new GameState();
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Path.Combine(Application.persistentDataPath, "/savedGames.acm"));
        bf.Serialize(file, SaveLoad.savedGames);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.acm"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Path.Combine(Application.persistentDataPath, "/savedGames.acm"), FileMode.Open);
            SaveLoad.savedGames = (List<GameState>)bf.Deserialize(file);
            file.Close();

            savedGames[0].Load();
        }
    }
}
