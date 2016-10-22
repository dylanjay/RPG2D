using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    Player player;

	void Start () {
        player = Player.instance;
    }

	void Update () {
	
	}

    public void Save()
    {
        SaveLoad.Save();
    }

    public void Load()
    {
        SaveLoad.Load();
    }
}
