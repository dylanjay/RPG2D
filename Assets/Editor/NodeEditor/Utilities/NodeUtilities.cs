using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

public static class NodeUtilities
{
#if UNITY_EDITOR
    public static string currentGraphPath;

    public static NodeGraph CreateNodeGraph(string graphName)
    {
        NodeGraph currentGraph = ScriptableObject.CreateInstance<NodeGraph>();
        if(currentGraph != null)
        {
            currentGraph.graphName = graphName;
            currentGraph.Initialize();
            //TODO: Prevent characters that cannot be in a file name from being typed in.
            AssetDatabase.CreateAsset(currentGraph, @"Assets/Resources/BehaviorTrees/" + graphName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
            if(currentWindow != null)
            {
                AskSaveGraph(currentWindow);
                currentGraphPath = "Assets/Resources/BehaviorTrees/" + graphName + ".asset";
                currentWindow.currentGraph = currentGraph;
            }
        }
        return currentGraph;
    }

    static void AskSaveGraph(NodeEditorWindow currentWindow)
    {
        if (currentWindow.currentGraph != null)
        {
            if (EditorUtility.DisplayDialog("Save Graph", "Save the current graph?", "Yes", "No"))
            {
                SaveGraph();
            }
        }
    }

    public static void LoadGraph()
    {
        NodeGraph currentGraph;
        string path = EditorUtility.OpenFilePanel("Load Graph", Application.dataPath + @"/Resources/BehaviorTrees/", "asset");
        if(string.IsNullOrEmpty(path))
        {
            return;
        }
        int appPathLen = Application.dataPath.Length;
        string finalPath = path.Substring(appPathLen - 6);
        NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
        if (currentGraphPath != path)
        {
            currentGraph = AssetDatabase.LoadAssetAtPath(finalPath, typeof(NodeGraph)) as NodeGraph;
            currentGraph.Reset();

            if (currentGraph != null)
            {
                if (currentWindow != null)
                {
                    AskSaveGraph(currentWindow);
                    currentGraphPath = path;
                    currentWindow.currentGraph = currentGraph;
                }
                else
                {
                    ErrorMessage("The node editor window is lost, reopen it. Then try reloading the graph. ");
                }
            }
            else
            {
                ErrorMessage("The selected asset can't be loaded as a Node Graph. ");
            }
        }
    }

    static void TreeIsValid(bool valid, NodeBase node)
    {
        if(node == null)
        {
            valid = false;
            Debug.LogError("Root has not been set");
            return;
        }

        if(node.behaviorComponent == null)
        {
            valid = false;
            Debug.LogError(node.title + " has a Null Behavior");
            return;
        }

        if (!valid)
        {
            return;
        }
        node.BuildTree();
        if (node.output != null)
        {
            if (node.output.childNodes.Any())
            {
                foreach (NodeBase child in node.output.childNodes)
                {
                    TreeIsValid(valid, child);
                }
            }
        }
        node.SaveBehavior();
    }

    public static void SaveGraph()
    {
        NodeGraph savedGraph = AssetDatabase.LoadAssetAtPath(currentGraphPath, typeof(NodeGraph)) as NodeGraph;
        if(savedGraph != null)
        {
            NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
            if(currentWindow != null)
            {
                savedGraph = currentWindow.currentGraph;

                bool validTree = true;
                TreeIsValid(validTree, savedGraph.rootNode);
                if (validTree)
                {
                    if (savedGraph.rootNode.behaviorComponent != null && !AssetDatabase.Contains(savedGraph.rootNode.behaviorComponent))
                    {
                        AssetDatabase.AddObjectToAsset(savedGraph.rootNode.behaviorComponent, savedGraph);
                    }
                    Debug.Log("Tree successfully created with root as " + savedGraph.rootNode.title);
                }
                else
                {
                    Debug.Log("Tree not created");
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                ErrorMessage("The node editor window is lost, reopen it. Then try reloading the graph.");
            }
        }
        else
        {
            ErrorMessage("The selected asset can't be loaded as a Node Graph.");
        }
    }

    public static void ClearGraph()
    {
        NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
        if (currentWindow != null)
        {
            if (EditorUtility.DisplayDialog("Clear Graph", "Are you sure you want to clear the current graph?", "Yes", "No"))
            {
                string graphName = currentWindow.currentGraph.graphName;
                AssetDatabase.DeleteAsset(currentGraphPath);
                CreateNodeGraph(graphName);
            }
        }
    }

    public static void UnLoadGraph()
    {
        NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
        if (currentWindow != null)
        {
            if (EditorUtility.DisplayDialog("Un-Load Graph", "Are you sure you want to un-load the current graph?", "Yes", "No"))
            {
                currentWindow.currentGraph = null;
            }
        }
    }

    public static void DeleteGraph()
    {
        if (EditorUtility.DisplayDialog("Delete Graph", "Are you sure you want to delete the current graph?", "Yes", "No"))
        {
            AssetDatabase.DeleteAsset(currentGraphPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public static void CreateNode(Type nodeType, BehaviorComponent behaviorComponent, NodeGraph graph, Vector2 position)
    {
        if(graph == null) { return; }

        NodeBase currentNode;
        string name;
        currentNode = NodeBase.CreateNode(nodeType);
        currentNode.behaviorComponent = behaviorComponent;

        if (behaviorComponent == null)
        {
            name = currentNode.name;
        }
        else
        {
            name = behaviorComponent.name;
        }
        currentNode.title = name;
        currentNode.name = name;
        
        if (currentNode != null)
        {
            currentNode.Initialize();
            currentNode.nodeRect.x = position.x;
            currentNode.nodeRect.y = position.y;
            currentNode.parentGraph = graph;
            graph.nodes.Add(currentNode);

            AssetDatabase.AddObjectToAsset(currentNode, graph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            /*if(currentNode.GetType() == typeof(NodeLeaf))
            {
                ((NodeLeaf)currentNode).Reset();
            }*/
        }
    }

    private static Type[] validTypes;
    private static Type[] sharedVariableDerivedTypes;
    private static GUIContent[] validTypeOptions;

    public static void InitializeValidTypes()
    {
        if(validTypeOptions == null)
        {
            validTypes =
                (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSubClassOfGeneric(typeof(SharedVariable<>))
                select type.BaseType.GetGenericArguments()[0]).ToArray();

            validTypeOptions =
                (from type in validTypes
                select new GUIContent(type.Name)).ToArray();
            sharedVariableDerivedTypes =
                (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSubClassOfGeneric(typeof(SharedVariable<>))
                select type).ToArray();
        }
        
    }

    public static GUIContent[] GetValidTypeOptions()
    {
        if(validTypes == null)
        {
            InitializeValidTypes();
        }
        return validTypeOptions;
    }

    public static Type[] GetValidTypes()
    {
        if (validTypes == null)
        {
            InitializeValidTypes();
        }
        return validTypes;
    }

    public static Type[] GetSharedVariableDerivedTypes()
    {
        if (validTypes == null)
        {
            InitializeValidTypes();
        }
        return sharedVariableDerivedTypes;
    }

#endif

    static void ErrorMessage(string body)
    {
        EditorUtility.DisplayDialog("Error", body, "OK");
    }

    public static void LoadGraph(string graphName)
    {

    }
}
