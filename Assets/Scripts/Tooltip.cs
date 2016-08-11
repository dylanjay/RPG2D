using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    private Item item;
    private string data;
    private GameObject tooltip;

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

    public void Activate(Item item)
    {
        this.item = item;
        ConstructDataString();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
    }

    public void ConstructDataString()
    {
        data = "<color=#36e7ff><b>" + item.Title + "</b></color>\n\n" +
            "<color=#ffb845>" + item.Description + "</color>\n\n" +
            "<color=#e9ff24>Value: " + item.Value + " gold</color>\n\n" +
            "Rarity: " + item.Rarity + "\n" +
            "Power: " + item.Power + "\n" +
            "Defence: " + item.Defence + "\n" +
            "Vitality: " + item.Vitality + "\n";
        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;

    }
}
