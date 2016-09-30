using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HealthBarManager : MonoBehaviour {

    public static HealthBarManager instance { get { return _instance; } }
    private static HealthBarManager _instance;

    GameObject prefab;

    void Awake()
    {
        _instance = this;
        prefab = Resources.Load("Prefabs/UI/HealthBar") as GameObject;
    }

	void Start ()
    {
        
	}
	
	void Update ()
    {
	
	}

    public GameObject Create()
    {
        GameObject bar = Instantiate(prefab, transform) as GameObject;
        bar.GetComponent<RectTransform>().localScale = prefab.GetComponent<RectTransform>().localScale;
        return bar;
    }
}
