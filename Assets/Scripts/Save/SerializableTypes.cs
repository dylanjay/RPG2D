﻿using UnityEngine;
using System.Collections.Generic;

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
        slots = SkillTree.instance.SaveSlots(slotSprites);
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
    public int tab;
    public string equipmentSlot;

    public void Fill(ItemData data)
    {
        id = data.item.id;
        stackAmount = data.stackAmount;
        slot = data.slot;
        tab = data.tab;
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

    public void Fill(List<List<GameObject>> itemList, Dictionary<Inventory.EquipmentType, Item> equipmentList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            for (int j = 0; j < itemList[i].Count; j++)
            {
                GameObject item = itemList[i][j];
                if (item.transform.childCount > 0)
                {
                    SerializableItem serItem = new SerializableItem();
                    serItem.Fill(item.transform.FindChild(item.name.Substring(0, item.name.Length - 5)).GetComponent<ItemData>());
                    items.Add(serItem);
                }
            }
        }

        foreach (KeyValuePair<Inventory.EquipmentType, Item> item in equipmentList)
        {
            SerializableItem serItem = new SerializableItem();
            serItem.Fill(item.Value.id, Inventory.EquipmentEnumToString(item.Key));
            equipment.Add(serItem);
        }
    }

    public void Load()
    {
        Inventory.instance.LoadInventory(items, equipment);
    }
}

[System.Serializable]
public class SerializablePlayerStats
{
    int playerLevel;
    int playerExp;
}
