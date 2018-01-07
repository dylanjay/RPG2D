using UnityEngine;
using UnityEditor;
using System;
using Benco.BehaviorTree;

namespace Benco.Graph
{
    //TODO(mderu): Get rid of all of this.
    public static class NodeUtilities
    {
        public static string currentGraphPath;

        public static T CreateNodeGraph<T>(string graphName) where T : NodeGraph
        {
            T currentGraph = ScriptableObject.CreateInstance<T>();

            currentGraph.name = graphName;
            currentGraph.Initialize();

            currentGraph.CreateAsset();

            NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
            if (currentWindow != null)
            {
                currentGraphPath = "Assets/Resources/" + graphName + ".asset";
                currentWindow.currentGraph = currentGraph;
                currentWindow.LoadGraph(currentGraph);
            }

            return currentGraph;
        }

        static bool TreeIsValid(BehaviorNodeBase node)
        {
            if (node == null)
            {
                Debug.LogError("Root has not been set.");
                return false;
            }

            if (node.behaviorComponent == null)
            {
                Debug.LogError(node.title + " has a Null Behavior");
                return false;
            }

            foreach (BehaviorNodeBase childNode in node.output.nodes)
            {
                if (!TreeIsValid(childNode))
                {
                    return false;
                }
            }
            return true;
        }

        // TODO(mderu): Get rid of this, or turn it into a baker.
        public static void SaveGraph()
        {
            NodeBehaviorTree savedGraph = AssetDatabase.LoadAssetAtPath<NodeBehaviorTree>(currentGraphPath);
            if (savedGraph != null)
            {
                NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
                if (currentWindow != null)
                {
                    savedGraph = (NodeBehaviorTree)currentWindow.currentGraph;

                    bool validTree = TreeIsValid((BehaviorNodeBase)savedGraph.root);
                    if (validTree)
                    {
                        BehaviorNodeBase behaviorRoot = ((BehaviorNodeBase)savedGraph.root);
                        if (behaviorRoot.behaviorComponent != null && !AssetDatabase.Contains(behaviorRoot.behaviorComponent))
                        {
                            AssetDatabase.AddObjectToAsset(behaviorRoot.behaviorComponent, savedGraph);
                        }
                        Debug.Log("Tree successfully created with root as " + savedGraph.root.title);
                    }
                    else
                    {
                        Debug.Log("Tree not created.");
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
                ErrorMessage("The selected asset can't be loaded as a NodeBehaviorTree.");
            }
        }
        
        public static void ClearGraph()
        {
            EditorUtility.DisplayDialog("Clear Graph", 
                "This function has been removed. I'll get around to implementing NodeGraph.Clear() later.", 
                "Okay, take your time.");
            //NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
            //if (currentWindow != null)
            //{
            //    if (EditorUtility.DisplayDialog("Clear Graph", "Are you sure you want to clear the current graph?", "Yes", "No"))
            //    {
            //        string graphName = currentWindow.currentGraph.name;
            //        AssetDatabase.DeleteAsset(currentGraphPath);
            //        CreateNodeGraph<NodeBehaviorTree>(graphName);
            //    }
            //}
        }

        public static void UnloadGraph()
        {
            NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
            if (currentWindow != null)
            {
                if (EditorUtility.DisplayDialog("Unload Graph", "Are you sure you want to un-load the current graph?", "Yes", "No"))
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

        public static void CreateNode(Type nodeType, BehaviorComponent nodeComponent, NodeGraph graph, Vector2 position)
        {
            if (graph == null) { return; }

            BehaviorNodeBase currentNode;
            currentNode = (BehaviorNodeBase)NodeBase.CreateNode(nodeType, graph, position);
            currentNode.behaviorComponent = nodeComponent;

            AssetDatabase.AddObjectToAsset(nodeComponent, graph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Undo.RegisterCreatedObjectUndo(nodeComponent, "Created Node Component");

            currentNode.title = nodeComponent == null ? currentNode.name : nodeComponent.name;
        }

        static void ErrorMessage(string body)
        {
            EditorUtility.DisplayDialog("Error", body, "OK");
        }
    }
}
