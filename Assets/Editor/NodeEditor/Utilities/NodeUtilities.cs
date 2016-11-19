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
            AssetDatabase.CreateAsset(currentGraph, @"Assets/Resources/BehaviorTrees/" + graphName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
            if(currentWindow != null)
            {
                AskSaveGraph(currentWindow);
                currentGraphPath = "Assets/Resources/BehaviorTrees/" + graphName + ".asset";
                currentWindow.currentGraph = currentGraph;
                CreateNode(currentGraph, typeof(NodeRoot), null, new Vector2(30, 50));
            }
        }
        return currentGraph;
    }

    static void AskSaveGraph(NodeEditorWindow currentWindow)
    {
        if (currentWindow.currentGraph != null)
        {
            if (EditorUtility.DisplayDialog("Save Graph", "Save current graph? ", "yes", "no"))
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
                foreach (NodeBase node in currentGraph.nodes)
                {
                    if (node.GetType() == typeof(NodeLeaf))
                    {
                        ((NodeLeaf)node).SetBehavior();
                    }
                }

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
        else
        {
            foreach (NodeBase node in currentWindow.currentGraph.nodes)
            {
                if (node.GetType() == typeof(NodeLeaf))
                {
                    ((NodeLeaf)node).SetBehavior();
                }
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

    static bool CreateTree(NodeBase node)
    {
        if (node.output != null)
        {
            if (node.output.childNodes.Any())
            {
                foreach (NodeBase child in node.output.childNodes)
                {
                    CreateTree(child);
                }
            }
        }
        
        if (!node.CreateTree())
        {
            return false;
        }
        return true;
    }

    static void CreateTreeAsset(NodeBase node, BehaviorComponent root)
    {   
        if(node.behaviorNode != root)
        {
            EditorUtility.SetDirty(node.behaviorNode);
            AssetDatabase.AddObjectToAsset(node.behaviorNode, root);
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
                NodeBase root = null;

                foreach (NodeBase node in savedGraph.nodes)
                {
                    if (node.GetType() == typeof(NodeRoot))
                    {
                        root = node;
                    }

                    if (node.GetType() == typeof(NodeLeaf))
                    {
                        ((NodeLeaf)node).SaveBehavior();
                    }
                }

                if (CreateTree(root))
                {
                    AssetDatabase.CreateAsset(root.behaviorNode, @"Assets/Resources/BehaviorTrees/" + savedGraph.name + "Tree" + ".asset");
                    CreateTreeAsset(root, root.behaviorNode);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
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

    public static void ClearGraph()
    {
        NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
        if (currentWindow != null)
        {
            if (EditorUtility.DisplayDialog("Clear Graph", "Are you sure you want to clear current graph? ", "yes", "no"))
            {
                currentWindow.currentGraph.nodes = new List<NodeBase>();
                CreateNode(currentWindow.currentGraph, typeof(NodeRoot), null, new Vector2(30, 50));
            }
        }
    }

    public static void DeleteGraph()
    {
        if (EditorUtility.DisplayDialog("Delete Graph", "Are you sure you want to delete current graph? ", "yes", "no"))
        {
            AssetDatabase.DeleteAsset(currentGraphPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public static void CreateNode(NodeGraph graph, Type nodeType, Type nodeSubType, Vector2 position)
    {
        if(graph == null) { return; }

        NodeBase currentNode;
        string name;
        if (nodeSubType == null)
        {
            currentNode = ScriptableObject.CreateInstance(nodeType) as NodeBase;
            name = nodeType.ToString();
            name = name.Substring(4);
        }
        else
        {
            currentNode = Activator.CreateInstance(nodeType, new object[] { nodeSubType }) as NodeBase;
            name = nodeSubType.ToString();
            name = name.Substring(8);
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
            
            //if want always save on new node
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
