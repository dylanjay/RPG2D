using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public enum Menu { Empty = 0, Main = 1, Inventory = 2, Skills = 3 }

    GameObject inventoryPanel;
    GameObject equipmentPanel;
    GameObject skillsPanel;
    GameObject mainMenuPanel;
    GameObject playerHealthBar;
    GameObject playerManaBar;

    Transform slotPanel;
    Tooltip tooltip;

    Menu activeMenu = Menu.Empty;

    void Start ()
    {
        inventoryPanel = transform.FindChild("Inventory").FindChild("Inventory Panel").gameObject;
        equipmentPanel = transform.FindChild("Inventory").FindChild("Equipment Panel").gameObject;
        skillsPanel = transform.FindChild("Skill Tree").FindChild("Skill Tree Panel").gameObject;
        mainMenuPanel = transform.FindChild("Main Menu Panel").gameObject;
        playerHealthBar = transform.FindChild("Bottom Bar").FindChild("Health Bar").gameObject;
        playerManaBar = transform.FindChild("Bottom Bar").FindChild("Mana Bar").gameObject;

        slotPanel = inventoryPanel.transform.FindChild("Inventory Slot Panel");
        tooltip = GetComponent<Tooltip>();
    }

    void Update()
    {
        Menu menuQueued = Menu.Empty;

        if (Input.GetButtonDown("Main Menu"))
        {
            menuQueued = Menu.Main;
        }

        else if (Input.GetButtonDown("Inventory"))
        {
            menuQueued = Menu.Inventory;
        }

        else if (Input.GetButtonDown("Skills"))
        {
            menuQueued = Menu.Skills;
        }

        else if (Input.GetButtonDown("Escape"))
        {
            MenuCloser();
        }

        if (activeMenu == Menu.Empty)
        {
            MenuOpener(menuQueued);
        }

        else if (activeMenu != Menu.Empty && menuQueued == activeMenu)
        {
            MenuCloser();
        }

        else if (activeMenu != Menu.Empty && menuQueued != Menu.Empty)
        {
            MenuCloser();
            MenuOpener(menuQueued);
        }
    }

    void MenuCloser()
    {
        switch (activeMenu)
        {
            case Menu.Main:
                mainMenuPanel.SetActive(false);
                break;

            case Menu.Inventory:
                if (tooltip.tooltipReference.activeSelf)
                {
                    tooltip.Deactivate();
                }

                if (slotPanel.GetChild(slotPanel.childCount - 1).GetComponent<ItemData>())
                {
                    slotPanel.GetChild(slotPanel.childCount - 1).GetComponent<ItemData>().Reset();
                }

                inventoryPanel.SetActive(false);
                equipmentPanel.SetActive(false);
                break;

            case Menu.Skills:
                skillsPanel.SetActive(false);
                playerHealthBar.SetActive(true);
                playerManaBar.SetActive(true);
                break;
        }
        activeMenu = Menu.Empty;
    }

    void MenuOpener(Menu menuQueued)
    {
        activeMenu = menuQueued;

        switch (activeMenu)
        {
            case Menu.Main:
                mainMenuPanel.SetActive(true);
                break;

            case Menu.Inventory:
                inventoryPanel.SetActive(true);
                equipmentPanel.SetActive(true);
                break;

            case Menu.Skills:
                skillsPanel.SetActive(true);
                playerHealthBar.SetActive(false);
                playerManaBar.SetActive(false);
                break;
        }
    } 
}
