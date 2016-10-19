using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class SkillTree : MonoBehaviour
{

    AbilityManager abilityManager;
    Transform skillTree;
    Transform activeSkills;
    Player player;
    GameObject playerGO;
    //bool slotSelected = false;
    int slotID;

    void Start()
    {
        //TODO initialize skill tree with prefab (based on class?)
        abilityManager = transform.parent.GetComponent<AbilityManager>();
        skillTree = GameObject.Find("Skill Tree Panel").transform;
        activeSkills = skillTree.FindChild("Active Skill Slots");
        skillTree = skillTree.FindChild("Skill Tree Slots");
        player = GetComponentInParent<Player>();
        playerGO = GameObject.Find("Player");
        //TODO enable abilities from save point
    }

    void Update()
    {
        /*if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);
                
            }
        }*/
    }

    /*public void OnPointerDown(PointerEventData eventData)
    {
        Transform initial = eventData.hovered[0].transform;
        Debug.Log(initial.name);
        if (initial.parent == activeSkills)
        {
            slotSelected = true;
            slotID = int.Parse(initial.name.Substring(5));
            initial.GetComponent<UnityEngine.UI.Image>().color = Color.yellow;
        }

        else if (slotSelected)
        {
            if (initial.parent == skillTree)
            {
                string skillName = initial.GetChild(0).name;
                abilityManager.EnableAbility(abilityManager.GetAbility(skillName));
                Transform skill = activeSkills.GetChild(slotID).GetChild(0);
                if (!skill.gameObject.activeSelf)
                {
                    skill.gameObject.SetActive(true);
                }
                skill.name = skillName;
                skill.GetComponent<UnityEngine.UI.Image>().sprite = initial.GetComponent<UnityEngine.UI.Image>().sprite;
            }
            initial.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            slotSelected = false;
        }
    }*/

    public void selectSlot()
    {
        Transform initial = EventSystem.current.currentSelectedGameObject.transform;
        slotID = int.Parse(initial.name.Substring(5));
    }

    public void assignSlot()
    {
        Transform initial = EventSystem.current.currentSelectedGameObject.transform;
        string skillName = initial.GetChild(0).name;
        abilityManager.EnableAbility(abilityManager.abilityDict[skillName], playerGO);
        Transform skill = activeSkills.GetChild(slotID).GetChild(0);
        if (!skill.gameObject.activeSelf)
        {
            skill.gameObject.SetActive(true);
        }
        skill.name = skillName;
        skill.GetComponent<UnityEngine.UI.Image>().sprite = initial.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite;
    }
}
