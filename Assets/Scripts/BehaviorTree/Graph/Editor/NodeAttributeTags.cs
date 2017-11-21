using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtensionMethods;
using UnityEditor;

using BehaviorComponent = Benco.BehaviorTree.BehaviorComponent;

namespace Benco.Graph
{
    /// <summary>
    /// This is the info that is shown when modifying an INode. There is a dropdown list of possible nodes that can
    /// be chosen from, as well as a GenericMenu that will sort them into a neater path to make selection easier
    /// for a much larger amount of nodes.
    /// </summary>
    public class NodeInfo
    {
        //Needs to be separated from SineInfo[] so it can be passed into dropdown calls.
        public GUIContent[] dropdownOptions { get; private set; }
        public SineInfo[] sineInfos { get; private set; }
        public GenericMenu nodeMenu { get; private set; }

        public NodeInfo()
        {
            sineInfos = new SineInfo[0];
            dropdownOptions = new GUIContent[0];
            nodeMenu = new GenericMenu();
        }

        public void AddInfo(SineInfo sineInfo)
        {
            sineInfos = sineInfos.CopyAdd(sineInfo).OrderBy(x => x.displayName).ToArray();
            GUIContent newElement = new GUIContent(sineInfo.displayName);
            dropdownOptions = dropdownOptions.CopyAdd(newElement).OrderBy(x => x.text).ToArray();
            GenericMenu.MenuFunction lambda = () =>
            {
                //TODO: BehaviorComponents shouldn't be in Benco.Graph framework code.
                BehaviorComponent component = BehaviorComponent.CreateComponent(sineInfo.classType);
                NodeUtilities.CreateNode(sineInfo.nodeType,
                                         component,
                                         NodeEditorWindow.instance.currentGraph,
                                         //TODO(P3): I don't like the dependency on GraphController here.
                                         NodeEditorWindow.graphController.lastMouseEvent.mousePosition);
            };
            nodeMenu.AddItem(new GUIContent(sineInfo.path), false, lambda);
        }
    }

    /// <summary>
    /// Sine: Show In Node Editor
    /// </summary>
    public class SineInfo
    {
        public Type classType;
        public Type nodeType;
        public string displayName;
        public string path;

        public SineInfo(Type classType, Type nodeType, string displayName, string path)
        {
            this.classType = classType;
            this.nodeType = nodeType;
            this.displayName = displayName;
            this.path = path;
        }
    }

    public static class NodeAttributeTags
    {
        private static Dictionary<Type, NodeInfo> nodeTypeToAttr;

        private static void Initialize()
        {
            nodeTypeToAttr = new Dictionary<Type, NodeInfo>();

            List<Type> iNodeTypeList = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                        from type in assembly.GetTypes()
                                        where type.GetInterfaces().Contains(typeof(INode))
                                        select type).ToList();


            foreach (Type classType in iNodeTypeList)
            {
                if (classType.IsAbstract) { continue; }

                Type nodeType = null;

                string displayName = "";
                Stack<string> menuPathStack = new Stack<string>();
                
                for (Type ancestor = classType; ancestor != typeof(object); ancestor = ancestor.BaseType)
                {
                    NodeTypeAttribute ntAttr = ancestor.GetAttribute<NodeTypeAttribute>();
                    bool nameEmpty = displayName == "";
                    if (ntAttr != null)
                    {
                        if (nameEmpty)
                        {
                            displayName = ntAttr.displayName;
                        }
                        menuPathStack.Push(ntAttr.menuPath);
                        if (nodeType == null)
                        {
                            nodeType = ntAttr.nodeType;
                        }
                    }
                    ShowInNodeEditorAttribute sineAttr = ancestor.GetAttribute<ShowInNodeEditorAttribute>();
                    if (sineAttr != null)
                    {
                        sineAttr.displayName = sineAttr.displayName == "" ? classType.Name : sineAttr.displayName;
                        if (nameEmpty)
                        {
                            displayName = sineAttr.displayName;
                        }
                        menuPathStack.Push(sineAttr.menuPath);
                    }
                }
                StringBuilder pathBuilder = new StringBuilder();
                foreach (string currentPath in menuPathStack)
                {
                    if (currentPath == "") { Debug.Log("Empty"); }

                    string appending = "{0}/";
                    if (currentPath.StartsWith(appending))
                    {
                        int offset = pathBuilder.Length == 0 || pathBuilder[pathBuilder.Length - 1] == '/' ? 0 : -1;
                        pathBuilder.Append(currentPath.Substring(appending.Length + offset));
                    }
                    else
                    {
                        pathBuilder.Remove(0, pathBuilder.Length);
                        pathBuilder.Append(currentPath);
                    }
                }

                SineInfo sineInfo = new SineInfo(classType, nodeType, displayName, pathBuilder.ToString());

                NodeInfo nodeInfo;

                if (nodeType == null)
                {
                    Debug.LogError("Error: type " + classType + " must have a node type associated with it.");
                    return;
                }

                if (nodeTypeToAttr.TryGetValue(nodeType, out nodeInfo))
                {
                    nodeInfo.AddInfo(sineInfo);
                }
                else
                {
                    nodeInfo = new NodeInfo();
                    nodeInfo.AddInfo(sineInfo);
                    nodeTypeToAttr.Add(nodeType, nodeInfo);
                }
            }

            //List<string> duplicateDisplayNamePairs =
            //    (from attr1 in attributes
            //     from attr2 in attributes
            //     where attr1.displayName == attr2.displayName && attr1 != attr2
            //     select attr1.classType.ToString() + " and " + attr2.classType.ToString()).ToList();
            ////attributes.Where(p => attributes.Any(l => p.displayName == l.displayName)).ToList();
            //foreach (string duplicatePair in duplicateDisplayNamePairs)
            //{
            //    Debug.LogError("Error: All ShowInNodeEditor display names must be unique! " + duplicatePair + " share the same displayName");
            //}
        }

        public static GenericMenu GetNodeMenu<T>()
        {
            if (nodeTypeToAttr == null) { Initialize(); }
            return nodeTypeToAttr[typeof(T)].nodeMenu;
        }

        public static SineInfo[] GetInfo(Type nodeType)
        {
            if (nodeTypeToAttr == null) { Initialize(); }

            NodeInfo nodeInfo;
            if (!nodeTypeToAttr.TryGetValue(nodeType, out nodeInfo))
            {
                return new SineInfo[0];
            }
            return nodeInfo.sineInfos;
        }

        public static Type GetType(Type nodeType, int optionIndex)
        {
            if (nodeTypeToAttr == null) { Initialize(); }
            return nodeTypeToAttr[nodeType].sineInfos[optionIndex].classType;
        }

        public static GUIContent[] GetNodeOptions(Type nodeType)
        {
            if (nodeTypeToAttr == null) { Initialize(); }
            return nodeTypeToAttr[nodeType].dropdownOptions;
        }
    }
}
