using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtensionMethods;
using UnityEditor;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Benco.Graph
{
    /// <summary>
    /// This is the info that is shown when modifying an INode. There is a dropdown list of possible nodes that can
    /// be chosen from, as well as a GenericMenu that will sort them into a neater path to make selection easier
    /// for a much larger amount of nodes.
    /// </summary>
    public class NodeInfo
    {
        private bool dropdownOptionsDirty = true;
        private GUIContent[] _dropdownOptions;
        public GUIContent[] dropdownOptions
        {
            get
            {
                if (dropdownOptionsDirty)
                {
                    _dropdownOptions = new GUIContent[_sineInfos.Count];
                    for (int i = 0; i < _sineInfos.Count; i++)
                    {
                        _dropdownOptions[i] = new GUIContent(_sineInfos[i].displayName);
                    }
                    dropdownOptionsDirty = false;
                }
                return _dropdownOptions;
            }
            private set { _dropdownOptions = value; }
        }

        private List<SineInfo> _sineInfos = new List<SineInfo>();
        public ReadOnlyCollection<SineInfo> sineInfos { get { return _sineInfos.AsReadOnly(); } }

        public void AddInfo(SineInfo sineInfo)
        {
            _sineInfos.InsertSorted(sineInfo, (x) => x.displayName);
            dropdownOptionsDirty = true;
        }
        
        public GenericMenu GetMenu(GenericMenu.MenuFunction2 menuFunction)
        {
            GenericMenu nodeMenu = new GenericMenu();
            for (int i = 0; i < _sineInfos.Count; i++)
            {
                SineInfo sineInfo = _sineInfos[i];
                nodeMenu.AddItem(new GUIContent(sineInfo.path), false, menuFunction, sineInfo);
            }
            return nodeMenu;
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

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> iNodeTypeList = (from assembly in assemblies
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
                            nodeType = (from assembly in assemblies
                                        where assembly.GetType(ntAttr.nodeTypeName) != null
                                        select assembly.GetType(ntAttr.nodeTypeName)).FirstOrDefault();
                            if (nodeType == null)
                            {
                                Debug.LogErrorFormat("Unable to find NodeType \"{0}\" for class {1}.",
                                                     ntAttr.nodeTypeName, ancestor.Name);
                            }
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

        public static GenericMenu GetNodeMenu<T>(GenericMenu.MenuFunction2 menuFunction)
        {
            if (nodeTypeToAttr == null) { Initialize(); }
            return nodeTypeToAttr[typeof(T)].GetMenu(menuFunction);
        }

        public static ReadOnlyCollection<SineInfo> GetInfo(Type nodeType)
        {
            if (nodeTypeToAttr == null) { Initialize(); }

            NodeInfo nodeInfo;
            if (!nodeTypeToAttr.TryGetValue(nodeType, out nodeInfo))
            {
                return new List<SineInfo>().AsReadOnly();
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
