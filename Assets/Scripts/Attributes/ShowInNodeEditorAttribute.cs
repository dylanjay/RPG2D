using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;

[AttributeUsage(AttributeTargets.Class)]
public class ShowInNodeEditorAttribute : Attribute
{
    public readonly bool showInToolbar;
    public readonly string displayName;
    public readonly bool generic;

    public ShowInNodeEditorAttribute(string displayName, bool showInToolbar)
    {
        this.showInToolbar = showInToolbar;
        this.displayName = displayName;
    }    
}

public enum NodeType { Composite, Decorator, Leaf }

public static class NodeEditorTags
{
    /// <summary>
    /// Sine: Show In Node Editor
    /// </summary>
    private class SineInfo
    {
        public Type classType;
        public bool showInToolbar;
        public string displayName;
        public SineInfo(Type classType, string displayName, bool showInToolbar)
        {
            this.classType = classType;
            this.showInToolbar = showInToolbar;
            this.displayName = displayName;
        }
    }
    static GUIContent[] _allComposites, _allDecorators, _allLeaves;
    static List<Type> _allCompositeTypes, _allDecoratorTypes, _allLeafTypes;

    static GUIContent[] _toolbarComposites, _toolbarDecorators,  _toolbarLeaves;
    static List<Type> _toolbarCompositeTypes, _toolbarDecoratorTypes, _toolbarLeafTypes;

    private static bool initialized = false;
    private static void Initialize()
    {
        initialized = true;
        List<SineInfo> attributes = new List<SineInfo>();
        {
            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in allTypes)
            {
                object[] attrArray = type.GetCustomAttributes(typeof(ShowInNodeEditorAttribute), true);
                if (attrArray.Length > 0)
                {
                    ShowInNodeEditorAttribute attribute = ((ShowInNodeEditorAttribute[])attrArray)[0];
                    attributes.Add(new SineInfo(type, attribute.displayName, attribute.showInToolbar));
                    Debug.Assert
                    (
                        typeof(BehaviorComponent).IsAssignableFrom(type) && type != typeof(BehaviorComponent),
                        "Error: ShowInNodeEditor tags an object that is not derived from BehaviorComponent"
                    );
                }
            }
        }

        List<string> duplicateDisplayNamePairs =
            (from attr1 in attributes
             from attr2 in attributes
             where attr1.displayName == attr2.displayName && attr1 != attr2
             select attr1.classType.ToString() + " and " + attr2.classType.ToString()).ToList();
            //attributes.Where(p => attributes.Any(l => p.displayName == l.displayName)).ToList();
        foreach(string duplicatePair in duplicateDisplayNamePairs)
        {
            Debug.LogError("Error: All ShowInNodeEditor display names must be unique! " + duplicatePair + " share the same displayName");
        } 

        _allComposites = (from attribute in attributes
                          where typeof(BehaviorComposite).IsAssignableFrom(attribute.classType)
                          select new GUIContent(attribute.displayName)).ToArray();

        _toolbarComposites = (from attribute in attributes
                              where typeof(BehaviorComposite).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                              select new GUIContent(attribute.displayName)).ToArray();

        _allCompositeTypes = (from attribute in attributes
                              where typeof(BehaviorComposite).IsAssignableFrom(attribute.classType)
                              select attribute.classType).ToList();

        _toolbarCompositeTypes = (from attribute in attributes
                                  where typeof(BehaviorComposite).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                                  select attribute.classType).ToList();


        _allDecorators = (from attribute in attributes
                          where typeof(BehaviorDecorator).IsAssignableFrom(attribute.classType)
                          select new GUIContent(attribute.displayName)).ToArray();

        _toolbarDecorators = (from attribute in attributes
                              where typeof(BehaviorDecorator).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                              select new GUIContent(attribute.displayName)).ToArray();

        _allDecoratorTypes = (from attribute in attributes
                              where typeof(BehaviorDecorator).IsAssignableFrom(attribute.classType)
                              select attribute.classType).ToList();

        _toolbarDecoratorTypes = (from attribute in attributes
                                  where typeof(BehaviorDecorator).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                                  select attribute.classType).ToList();

        _allLeaves = (from attribute in attributes
                      where typeof(BehaviorLeaf).IsAssignableFrom(attribute.classType)
                      select new GUIContent(attribute.displayName)).ToArray();

        _toolbarLeaves = (from attribute in attributes
                          where typeof(BehaviorLeaf).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                          select new GUIContent(attribute.displayName)).ToArray();

        _allLeafTypes = (from attribute in attributes
                         where typeof(BehaviorLeaf).IsAssignableFrom(attribute.classType)
                         select attribute.classType).ToList();

        _toolbarLeafTypes = (from attribute in attributes
                             where typeof(BehaviorLeaf).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                             select attribute.classType).ToList();
    }

    public static GUIContent[] GetAllLabelsOfNodeType(NodeType nodeType)
    {
        if (!initialized) { Initialize(); }
        switch (nodeType)
        {
            case NodeType.Composite:
                return (GUIContent[])_allComposites.Clone();
            case NodeType.Decorator:
                return (GUIContent[])_allDecorators.Clone();
            case NodeType.Leaf:
                return (GUIContent[])_allLeaves.Clone();
            default:
                Debug.LogError("Error: Unknown NodeType " + nodeType + ".");
                return new GUIContent[0];
        }
    }
    
    public static ReadOnlyCollection<Type> GetAllTypesOfNodeType(NodeType nodeType)
    {
        if (!initialized) { Initialize(); }
        switch (nodeType)
        {
            case NodeType.Composite:
                return _allCompositeTypes.AsReadOnly();
            case NodeType.Decorator:
                return _allDecoratorTypes.AsReadOnly();
            case NodeType.Leaf:
                return _allLeafTypes.AsReadOnly();
            default:
                Debug.LogError("Error: Unknown NodeType " + nodeType + ".");
                return new ReadOnlyCollection<Type>(new List<Type>());
        }
    }

    public static GUIContent[] GetToolbarLabelsOfNodeType(NodeType nodeType)
    {
        if (!initialized) { Initialize(); }
        switch (nodeType)
        {
            case NodeType.Composite:
                return (GUIContent[])_toolbarComposites.Clone();
            case NodeType.Decorator:
                return (GUIContent[])_toolbarDecorators.Clone();
            case NodeType.Leaf:
                return (GUIContent[])_toolbarLeaves.Clone();
            default:
                Debug.LogError("Error: Unknown NodeType " + nodeType + ".");
                return new GUIContent[0];
        }
    }

    public static ReadOnlyCollection<Type> GetToolbarTypesOfNodeType(NodeType nodeType)
    {
        if (!initialized) { Initialize(); }
        switch (nodeType)
        {
            case NodeType.Composite:
                return _toolbarCompositeTypes.AsReadOnly();
            case NodeType.Decorator:
                return _toolbarDecoratorTypes.AsReadOnly();
            case NodeType.Leaf:
                return _toolbarLeafTypes.AsReadOnly();
            default:
                Debug.LogError("Error: Unknown NodeType " + nodeType + ".");
                return new ReadOnlyCollection<Type>(new List<Type>());
        }
    }
}