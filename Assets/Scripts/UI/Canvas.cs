using UnityEngine;
using System.Collections;

public class Canvas : MonoBehaviour
{
    public static Canvas instance { get; private set; }

    Inventory inventory;
    SkillTree skillTree;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inventory = Inventory.instance;
        skillTree = SkillTree.instance;
    }
}
