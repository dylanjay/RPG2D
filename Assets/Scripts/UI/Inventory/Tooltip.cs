using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    Item item;
    string data;

    public GameObject tooltipReference;

    void Start()
    {
        tooltipReference.SetActive(false);
    }

    void Update()
    {
        if(tooltipReference.activeSelf)
        {
            tooltipReference.transform.position = Input.mousePosition;
        }
    }

    public void Activate(Item hoveredItem)
    {
        item = hoveredItem;
        ConstructDataString();
        tooltipReference.SetActive(true);
    }

    public void Deactivate()
    {
        tooltipReference.SetActive(false);
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

        tooltipReference.transform.GetChild(0).GetComponent<Text>().text = data;

    }
}
