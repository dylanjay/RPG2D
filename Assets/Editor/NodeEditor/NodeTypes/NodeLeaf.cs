using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class NodeLeaf : NodeBase
{
    private static GUIContent[] _allLeafOptions = null;
    private static ReadOnlyCollection<Type> _allLeafTypes = null;

    public override GUIContent[] GetAllBehaviorOptions()
    {
        if (_allLeafOptions == null)
        {
            GUIContent[] tmp = NodeEditorTags.GetAllLabelsOfNodeType(NodeType.Leaf);
            _allLeafOptions = new GUIContent[tmp.Length + 1];
            _allLeafOptions[0] = new GUIContent("None");
            for (int i = 0; i < tmp.Length; i++)
            {
                _allLeafOptions[i + 1] = tmp[i];
            }
        }
        return _allLeafOptions;
    }

    public override ReadOnlyCollection<Type> GetAllBehaviorTypes()
    {
        if (_allLeafTypes == null)
        {
            _allLeafTypes = NodeEditorTags.GetAllTypesOfNodeType(NodeType.Leaf);
        }
        return _allLeafTypes;
    }

    private int optionNumber = 0;

    public NodeLeaf()
    {
        input = new NodeInput();
    }

    public override void Initialize()
    {
        base.Initialize();
        nodeRect = new Rect(10, 10, 150, 35);
    }

    //TODO: Fix saving
    public void SaveBehavior()
    {
        if (behaviorComponent != null)
        {
            string pathToTree = @"Assets/Resources/BehaviorTrees/" + "ConcreteTree" + ".asset";
            AssetDatabase.AddObjectToAsset(behaviorComponent, pathToTree);
            /*for(int i = 1;  i < constructor.GetParameters().Length; i++)
            {
                UnityEngine.Object obj = constructorValues[i] as UnityEngine.Object;
                AssetDatabase.AddObjectToAsset(obj, pathToBehavior);
            }*/
        }
    }

    public override void UpdateNodeGUI(Event e, Rect viewRect)
    {
        base.UpdateNodeGUI(e, viewRect);
        if(output != null)
        {
            output = null;
        }
    }
#if UNITY_EDITOR
    public override void DrawNodeProperties()
    {
        base.DrawNodeProperties();
    }

    public override void DrawNodeHelp()
    {
        base.DrawNodeHelp();
    }
#endif
}
