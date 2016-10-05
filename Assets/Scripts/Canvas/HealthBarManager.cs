using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HealthBarManager : MonoBehaviour {

    public static HealthBarManager instance { get; private set; }

    [SerializeField]
    GameObject hostileHealthBarPrefab;

    [SerializeField]
    GameObject bossHealthBarReference;

    void Awake()
    {
        instance = this;
    }

	void Start ()
    {
        
	}
	
	void Update ()
    {
    }

    public void UpdateHealthBar(GameObject healthBar, MaxableStat health)
    {
        float healthBarDisplay = health.ratio;
        Transform healthBarFill = healthBar.transform.FindChild("Foreground").FindChild("Fill");
        Vector3 barScale = healthBarFill.GetComponent<RectTransform>().localScale;
        healthBarFill.GetComponent<RectTransform>().localScale = new Vector3(healthBarDisplay, barScale.y, barScale.z);
    }

    public GameObject RequestHealthBar(bool isBoss)
    {
        if(isBoss)
        {
            return bossHealthBarReference;
        }
        else
        {
            GameObject bar = Instantiate(hostileHealthBarPrefab, transform) as GameObject;
            bar.GetComponent<RectTransform>().localScale = hostileHealthBarPrefab.GetComponent<RectTransform>().localScale;
            return bar;
        }
    }
}
