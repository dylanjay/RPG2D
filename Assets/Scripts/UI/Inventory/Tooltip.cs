using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    Item item;
    string data;

    public GameObject tooltip;

    void Start()
    {
        tooltip = GameObject.Find("Tooltip");
        tooltip.SetActive(false);
    }

    void Update()
    {
        if(tooltip.activeSelf)
        {
            tooltip.transform.position = Input.mousePosition;
        }
    }

    public void Activate(Item hoveredItem)
    {
        item = hoveredItem;
        ConstructDataString();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
    }

    public void ConstructDataString()
    {
        data = "<color=#36e7ff><b>" + item.title + "</b></color>\n\n" +
            "<color=#ffb845>" + item.description + "</color>\n\n" +
            "<color=#e9ff24>Value: " + item.value + " gold</color>\n\n" +
            "Rarity: " + item.tier + "\n";
        for(int i = 0; i < item.stats.Count; i++)
        {
            data += item.stats[i].name + ": " + item.stats[i].value + "\n";
        }

        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;

    }
}
