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
        Debug.Log(currentGraphPath);
        Debug.Log(path);
        if (currentGraphPath != path)
        {
            currentGraph = AssetDatabase.LoadAssetAtPath(finalPath, typeof(NodeGraph)) as NodeGraph;

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

    /*static bool CreateTree(NodeBase node)
    {
        bool valid = true;
        if (node.output != null)
        {
            if (node.output.childNodes.Any())
            {
                foreach (NodeBase child in node.output.childNodes)
                {
                    valid = CreateTree(child);
                }
            }
        }

        if (valid)
        {
            valid = node.CreateTree();
        }
        else
        {
            node.CreateTree();
            valid = false;
        }
        EditorUtility.SetDirty(node.behaviorNode);
        return valid;
    }*/

    static void CreateTreeAsset(NodeBase node, BehaviorComponent root)
    {   
        if(node.behaviorComponent != root)
        {
            EditorUtility.SetDirty(node.behaviorComponent);
            AssetDatabase.AddObjectToAsset(node.behaviorComponent, root);
        }

        if (node.output != null)
        {
            if (node.output.childNodes.Any())
            {
                foreach (NodeBase child in node.output.childNodes)
                {
                    CreateTreeAsset(child, root);
                }
            }
        }
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

                foreach (NodeBase node in savedGraph.nodes)
                {
                    if (node.GetType() == typeof(NodeLeaf))
                    {
                        ((NodeLeaf)node).SaveBehavior();
                    }
                }

                //TODO: Actually Serialize.
                /*if (CreateTree(root))
                {
                    AssetDatabase.CreateAsset(root.behaviorNode, @"Assets/Resources/BehaviorTrees/" + savedGraph.name + "Tree" + ".asset");
                    CreateTreeAsset(root, root.behaviorNode);
                }*/

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
                currentWindow.currentGraph.nodes = new List<NodeBase>();
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
        }
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
