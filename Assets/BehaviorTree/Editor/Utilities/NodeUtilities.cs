using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Benco.BehaviorTree.TreeEditor
{
    public static class NodeUtilities
    {
#if UNITY_EDITOR
        public static string currentGraphPath;

        public static NodeBehaviorTree CreateNodeGraph(string graphName)
        {
            NodeBehaviorTree currentGraph = ScriptableObject.CreateInstance<NodeBehaviorTree>();
            if (currentGraph != null)
            {
                currentGraph.name = graphName;
                currentGraph.Initialize();

                currentGraph.CreateAsset();

                NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
                if (currentWindow != null)
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
            NodeBehaviorTree currentGraph;
            string path = EditorUtility.OpenFilePanel("Load Graph", Application.dataPath + @"/Resources/BehaviorTrees/", "asset");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            int appPathLen = Application.dataPath.Length;
            string finalPath = path.Substring(appPathLen - 6);
            NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
            if (currentGraphPath != path)
            {
                currentGraph = AssetDatabase.LoadAssetAtPath(finalPath, typeof(NodeBehaviorTree)) as NodeBehaviorTree;
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
            if (node == null)
            {
                valid = false;
                Debug.LogError("Root has not been set");
                return;

            }

            if (node.behaviorComponent == null)
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
            NodeBehaviorTree savedGraph = AssetDatabase.LoadAssetAtPath(currentGraphPath, typeof(NodeBehaviorTree)) as NodeBehaviorTree;
            if (savedGraph != null)
            {
                NodeEditorWindow currentWindow = EditorWindow.GetWindow<NodeEditorWindow>();
                if (currentWindow != null)
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
                    string graphName = currentWindow.currentGraph.name;
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

        public static void CreateNode(Type nodeType, BehaviorComponent behaviorComponent, NodeBehaviorTree graph, Vector2 position)
        {
            if (graph == null) { return; }

            NodeBase currentNode;
            currentNode = NodeBase.CreateNode(nodeType, graph, position);
            currentNode.behaviorComponent = behaviorComponent;

            if (behaviorComponent == null)
            {
                currentNode.title = currentNode.name;
            }
            else
            {
                currentNode.title = behaviorComponent.name;
            }
        }

        private static Type[] _validTypes;
        private static Type[] _sharedVariableDerivedTypes;
        private static GUIContent[] _validTypeOptions;

        public static void InitializeValidTypes()
        {
            if (_validTypeOptions == null)
            {
                _validTypes =
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.IsSubclassOf(typeof(SharedVariable)) && type.BaseType.IsGenericType
                     let attributes = (TypeNameOverrideAttribute[])type.GetCustomAttributes(typeof(TypeNameOverrideAttribute), false)
                     let name = attributes.Length > 0 ? attributes[0].newDisplayName : type.BaseType.GetGenericArguments()[0].Name
                     orderby name
                     select type.BaseType.GetGenericArguments()[0]).ToArray();

                _validTypeOptions =
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.IsSubclassOf(typeof(SharedVariable)) && type.BaseType.IsGenericType
                     let attributes = (TypeNameOverrideAttribute[])type.GetCustomAttributes(typeof(TypeNameOverrideAttribute), false)
                     let name = attributes.Length > 0 ? attributes[0].newDisplayName : type.BaseType.GetGenericArguments()[0].Name
                     orderby name
                     select new GUIContent(name)).ToArray();

                _sharedVariableDerivedTypes =
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.IsSubclassOf(typeof(SharedVariable)) && type.BaseType.IsGenericType
                     let attributes = (TypeNameOverrideAttribute[])type.GetCustomAttributes(typeof(TypeNameOverrideAttribute), false)
                     let name = attributes.Length > 0 ? attributes[0].newDisplayName : type.Name
                     orderby name
                     select type).ToArray();
            }
        }

        public static GUIContent[] validTypeOptions
        {
            get
            {
                if (_validTypeOptions == null)
                {
                    InitializeValidTypes();
                }
                return _validTypeOptions;
            }
        }

        public static Type[] validTypes
        {
            get
            {
                if (_validTypes == null)
                {
                    InitializeValidTypes();
                }
                return _validTypes;
            }
        }

        public static Type[] sharedVariableDerivedTypes
        {
            get
            {
                if (_sharedVariableDerivedTypes == null)
                {
                    InitializeValidTypes();
                }
                return _sharedVariableDerivedTypes;
            }
        }

        static void ErrorMessage(string body)
        {
            EditorUtility.DisplayDialog("Error", body, "OK");
        }
#endif
    }
}
