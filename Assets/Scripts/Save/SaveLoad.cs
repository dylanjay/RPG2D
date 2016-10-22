using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class GameState
{
    public static GameState current;
    //public GameObject player;
    public int test;


    public GameState()
    {
        //player = GameObject.Find("Player");
        test = 10;
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

            //GameObject player = GameObject.Find("Player");
            //player = SaveLoad.savedGames[0].player;
            Debug.Log(SaveLoad.savedGames[0].test);
        }
    }
}
