using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance { get; private set; }

    public int strength;
    public int endurance;
    public int charisma;
    public int intelligence;
    public int agility;

    Player player;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player = Player.instance;
    }
}
