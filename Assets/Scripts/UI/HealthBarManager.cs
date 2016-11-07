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

    [SerializeField]
    GameObject playerHealthBarReference;

    void Awake()
    {
        instance = this;
    }
    
    public void UpdateHealthBar(GameObject healthBar, MaxableStat health)
    {
        float healthBarDisplay = health.ratio;
        Transform healthBarFill = healthBar.transform.FindChild("Foreground").FindChild("Fill");
        Vector3 barScale = healthBarFill.GetComponent<RectTransform>().localScale;
        healthBarFill.GetComponent<RectTransform>().localScale = new Vector3(healthBarDisplay, barScale.y, barScale.z);
    }

    public GameObject RequestHealthBar()
    {
        GameObject bar = Instantiate(hostileHealthBarPrefab, transform) as GameObject;
        bar.GetComponent<RectTransform>().localScale = hostileHealthBarPrefab.GetComponent<RectTransform>().localScale;
        return bar;
    }

    public GameObject GetPlayerHealthBar()
    {
        return playerHealthBarReference;
    }

    public GameObject GetBossHealthBar()
    {
        return bossHealthBarReference;
    }
}
