using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
        FileStream file = File.Create(Path.Combine(Application.persistentDataPath, "/savedGames.tbd"));
        bf.Serialize(file, savedGames);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.acm"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Path.Combine(Application.persistentDataPath, "/savedGames.tbd"), FileMode.Open);
            savedGames = (List<GameState>)bf.Deserialize(file);
            file.Close();

            savedGames[0].Load();
        }
    }
}
