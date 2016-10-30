using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class InventoryTab : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Text tabNumberText;

    int tab;
    float switchTimer = 0.0f;
    float switchDuration = 1.0f;
    bool switchTimerOn = false;
    Inventory inventory;

    void Awake()
    {
        inventory = Inventory.instance;
    }

    void Update()
    {
        if(switchTimerOn)
        {
            switchTimer += Time.deltaTime;

            if (switchTimer >= switchDuration)
            {
                inventory.SwitchTabs(tab);
            }
        }
    }

    public void SetTabNumber(int num)
    {
        tab = num;
        tabNumberText.text = (num+1).ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        inventory.SwitchTabs(tab);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventory.holdingItem && inventory.currentTab != tab)
        {
            switchTimerOn = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switchTimer = 0.0f;
        switchTimerOn = false;
    }
}
