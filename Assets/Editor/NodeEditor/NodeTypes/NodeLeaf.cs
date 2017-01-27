using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

//using GUIContent = SGUIContent;

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


    public NodeLeaf()
    {
        input = new NodeInput();
    }

    public override void Reset()
    {
        base.Reset();
        if (output != null)
        {
            output = null;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        nodeRect = new Rect(10, 10, 150, 35);
    }

    public override void UpdateNodeGUI(Event e, Rect viewRect)
    {
        base.UpdateNodeGUI(e, viewRect);
        //TODO switch to reset function
        if (output != null)
        {
            output = null;
        }
    }
#if UNITY_EDITOR
    public override void DrawNodeProperties()
    {
        GUILayout.BeginVertical();
        {
            title = EditorGUILayout.TextField("Title", title);
            description = EditorGUILayout.TextField("Description", description);
        }
        GUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Space();

            int prevOptionNumber = behaviorComponent == null ? 0 : GetAllBehaviorTypes().IndexOf(behaviorComponent.GetType());
            BehaviorComponent tempComponent = EditorGUILayout.ObjectField("Type", behaviorComponent, typeof(BehaviorLeaf), false) as BehaviorComponent;
            int optionNumber = tempComponent == null ? 0 : GetAllBehaviorTypes().IndexOf(tempComponent.GetType());

            if (optionNumber != prevOptionNumber)
            {
                bool changeName = (behaviorComponent == null) ? title == name : title == behaviorComponent.name;

                if (prevOptionNumber != 0)
                {
                    parentGraph.RemoveBehavior(this);
                }

                DestroyImmediate(behaviorComponent, true);
                //Option 0 is "None", a null behaviorComponent
                if (optionNumber == 0)
                {
                    behaviorComponent = null;
                    if (changeName)
                    {
                        title = name;
                    }
                }
                else
                {
                    behaviorComponent = BehaviorComponent.CreateComponent(GetAllBehaviorTypes()[optionNumber]);
                    if (changeName)
                    {
                        title = behaviorComponent.name;
                    }
                }
                parentGraph.AddBehavior(this);

                foreach (FieldInfo fieldInfo in behaviorComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (((((Attribute[])fieldInfo.GetCustomAttributes(typeof(HideInInspector), true)).Length > 0) ||
                        (fieldInfo.IsPrivate && (((Attribute[])fieldInfo.GetCustomAttributes(typeof(SerializeField), true)).Length == 0))))
                    {
                        continue;
                    }
                    if (fieldInfo.FieldType.IsSubClassOfGeneric(typeof(SharedVariable<>)))
                    {
                        choices[fieldInfo.Name] = SharedVariableCollection.none;
                    }
                }
            }

            EditorGUILayout.Space();
        }
    }

    public override void DrawNodeHelp()
    {
        base.DrawNodeHelp();
    }
#endif
}