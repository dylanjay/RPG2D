using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using ExtensionMethods;
using System.Text.RegularExpressions;

public class BehaviorTreeMaker : MonoBehaviour {

    [SerializeField]
    private GameObject behaviorButtonPrefab;
    [SerializeField]
    private GameObject createBehaviorUI;
    [SerializeField]
    private GameObject uiButtonPrefab;
    private int numUsedButtons;
    private List<GameObject> pooledUIButtons = new List<GameObject>();

    [SerializeField]
    private GameObject rootNode;
    [SerializeField]
    private GameObject createBehaviorButtonHolder;

    private GameObject selectedBehaviorGO;

    string currentPath = "Assets/Scripts/BehaviorTree";
    
	void Start ()
    {

	}

    public void SetSelectedBehavior(GameObject button)
    {
        if(selectedBehaviorGO != null)
        {
            selectedBehaviorGO.GetComponent<Image>().color = new Color(1.0f,0.5f,0.0f);
        }
        button.GetComponent<Image>().color = Color.yellow;
        selectedBehaviorGO = button;
        BehaviorSlot selectedBehavior = button.GetComponent<BehaviorSlot>();

        ShowSetBehaviorUI(selectedBehavior);

        if (selectedBehavior != null)
        {

        }
    }

    private void ShowSetBehaviorUI(BehaviorSlot selectedBehavior)
    {
        ClearButtons();
        createBehaviorUI.SetActive(true);

        //if(selectedBehavior == null)
        {
            if (currentPath.IndexOf('\\') > 0)
            {
                GameObject buttonGO = GetButton();
                buttonGO.GetComponentInChildren<Text>().text = "..";
                buttonGO.GetComponent<Image>().color = new Color(.75f, .75f, 1);
                buttonGO.name = "..";
            }
            foreach (string s in Directory.GetDirectories(currentPath))
            {
                string dirName = s.Remove(0, s.LastIndexOf('\\') + 1);

                GameObject buttonGO = GetButton();
                buttonGO.GetComponentInChildren<Text>().text = dirName;
                buttonGO.GetComponent<Image>().color = new Color(.75f,.75f,1);
                buttonGO.name = string.Format("Directory {0}", dirName);
            }
            foreach (string s in Directory.GetFiles(currentPath))
            {
                if (s.LastIndexOf(".meta") >= 0) { continue; }
                string typeName = s.SubstringFromCharsSqueeze('\\', '.');
                Type type = Type.GetType(typeName);
                if (type == null || type.IsAbstract || !type.IsSubclassOf(typeof(BehaviorComponent)))
                {
                    continue;
                }
                else
                {
                    GameObject buttonGO = GetButton();
                    buttonGO.GetComponentInChildren<Text>().text = typeName;
                    buttonGO.name = string.Format("Type {0}", typeName);
                    if(selectedBehavior.GetComponent<BehaviorSlot>().behaviorComponent != null)
                    {
                        Type selectedButtonType = selectedBehavior.GetComponent<BehaviorSlot>().behaviorComponent.GetType();
                        if (selectedButtonType == type)
                        {
                            buttonGO.GetComponent<Image>().color = Color.yellow;
                        }
                    }
                    
                }
            }
        }
    }

    public void UIButtonClicked(GameObject clickedUIButton)
    {
        /*string buttonName = clickedUIButton.name.Substring(clickedUIButton.name.IndexOf(' ') + 1);

        //If it is a type
        if(clickedUIButton.name[0] == 'T')
        {
            Type behaviorType = Type.GetType(buttonName);

            clickedUIButton.GetComponent<Image>().color = Color.yellow;

            BehaviorSlot behaviorSlot = selectedBehaviorGO.GetComponent<BehaviorSlot>();

            UpdateCurrentNodeInHeirarchy(behaviorType);
        }
        //If it is a directory
        else
        {
            if (buttonName[0] == '.')
            {
                currentPath = currentPath.Remove(currentPath.LastIndexOf('\\'));
            }
            else
            {
                currentPath += string.Format("\\{0}", buttonName);
            }
            ShowSetBehaviorUI(selectedBehaviorGO.GetComponent<BehaviorSlot>());
        }*/
    }

    private void ClearButtons()
    {
        numUsedButtons = 0;
        for(int i = 0; i < pooledUIButtons.Count; i++)
        {
            pooledUIButtons[i].SetActive(false);
            pooledUIButtons[i].GetComponent<Image>().color = Color.white;
        }
    }

    /*private void UpdateCurrentNodeInHeirarchy(Type behaviorType)
    {
        BehaviorSlot selectedBSlot = selectedBehaviorGO.GetComponent<BehaviorSlot>();

        //First time this node has recieved a behavior.
        if (selectedBSlot.behaviorComponent == null)
        {
            selectedBSlot.behaviorComponent = (BehaviorComponent)ScriptableObject.CreateInstance(behaviorType);
            if (selectedBSlot.parentObject != null)
            {
                if (selectedBSlot.parentObject.GetComponent<BehaviorSlot>().behaviorComponent is BehaviorComposite)
                {
                    CreateNewNode(selectedBSlot.parentObject.transform);
                }
            }
            if (selectedBSlot.behaviorComponent is BehaviorComposite || selectedBSlot.behaviorComponent is BehaviorComposite)
            {
                GameObject childNode = CreateNewNode(selectedBehaviorGO.transform);
            }
        }
        //If the node is being changed.
        else if (selectedBSlot.behaviorComponent.GetType() != behaviorType)
        {

        }
        
    }*/

    private GameObject CreateNewNode(Transform parent)
    {
        GameObject childNode = Instantiate(behaviorButtonPrefab);
        childNode.GetComponent<BehaviorSlot>().parentObject = parent.gameObject;
        childNode.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedBehavior(childNode); });
        childNode.transform.SetParent(parent);
        childNode.transform.localPosition = Vector2.down * 75;



        /*BehaviorComponent parentBehavior = parent.GetComponent<BehaviorSlot>().behaviorComponent;
        if(parentBehavior is BehaviorComposite)
        {
            ((BehaviorComposite)parentBehavior).childBehaviors.Add(childNode.GetComponent<BehaviorSlot>().behaviorComponent);
        }
        else if(parentBehavior is BehaviorDecorator)
        {
            ((BehaviorDecorator)parentBehavior).childBehavior = (childNode.GetComponent<BehaviorSlot>().behaviorComponent);
        }
        else //if is a leaf
        {
            Debug.LogError("Tried to create a child of a leaf.");
        }*/

        return childNode;
    }

    private GameObject GetButton()
    {
        numUsedButtons++;
        if(pooledUIButtons.Count <= numUsedButtons)
        {
            GameObject newPooledUIButton = Instantiate(uiButtonPrefab);
            newPooledUIButton.GetComponent<Button>().onClick.AddListener( delegate { UIButtonClicked(newPooledUIButton); } );
            pooledUIButtons.Add(newPooledUIButton);
            newPooledUIButton.transform.SetParent(createBehaviorButtonHolder.transform, false);
            return newPooledUIButton;
        }
        else
        {
            pooledUIButtons[numUsedButtons].SetActive(true);
            return pooledUIButtons[numUsedButtons];
        }
    }

    void Update()
    {
        string inputString = Input.inputString;
        bool ctrlBackspace = inputString.Contains("\u007F");
        Regex rgx = new Regex("[^a-zA-Z0-9'$%!@#&*(),.><?/:\" -]");
        inputString = rgx.Replace(inputString, "");

        if (selectedBehaviorGO != null)
        {
            BehaviorSlot selectedBehaviorSlot = selectedBehaviorGO.GetComponent<BehaviorSlot>();
            if(selectedBehaviorSlot != null)
            {
                string curNodeName = selectedBehaviorGO.GetComponent<BehaviorSlot>().nodeName;
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (ctrlBackspace)
                    {
                        selectedBehaviorGO.GetComponent<BehaviorSlot>().nodeName = curNodeName.Substring(0, Mathf.Max(0, curNodeName.LastIndexOf(' ')));
                    }
                    else
                    {
                        selectedBehaviorGO.GetComponent<BehaviorSlot>().nodeName = curNodeName.Substring(0, Mathf.Max(0,curNodeName.Length - 1));
                    }
                }
                else
                {
                    selectedBehaviorGO.GetComponent<BehaviorSlot>().nodeName += inputString;
                }
            }
            
        }
        
    }
}
