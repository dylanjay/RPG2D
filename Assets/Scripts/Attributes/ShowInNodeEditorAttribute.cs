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

    public ShowInNodeEditorAttribute(bool showInToolbar)
    {
        this.showInToolbar = showInToolbar;
    }    
}

public static class NodeEditorTags
{
    /// <summary>
    /// Sine: Show In Node Editor
    /// </summary>
    private class SineInfo
    {
        public Type classType;
        public bool showInToolbar;
        public SineInfo(Type classType, bool showInToolbar)
        {
            this.classType = classType;
            this.showInToolbar = showInToolbar;
        }
    }
    static GUIContent[] _allComposites = new GUIContent[0];
    static GUIContent[] _allDecorators = new GUIContent[0];
    static GUIContent[] _allLeaves     = new GUIContent[0];
    static List<Type> _allCompositeTypes = new List<Type>();
    static List<Type> _allDecoratorTypes = new List<Type>();
    static List<Type> _allLeafTypes = new List<Type>();

    static GUIContent[] _toolbarComposites = new GUIContent[0];
    static GUIContent[] _toolbarDecorators = new GUIContent[0];
    static GUIContent[] _toolbarLeaves     = new GUIContent[0];
    static List<Type> _toolbarCompositeTypes = new List<Type>();
    static List<Type> _toolbarDecoratorTypes = new List<Type>();
    static List<Type> _toolbarLeafTypes = new List<Type>();

    public static GUIContent[] allComposites { get { if (!initialized) { Initialize(); } return (GUIContent[])_allComposites.Clone(); } }
    public static GUIContent[] allDecorators { get { if (!initialized) { Initialize(); } return (GUIContent[])_allDecorators.Clone(); } }
    public static GUIContent[] allLeaves     { get { if (!initialized) { Initialize(); } return (GUIContent[])_allLeaves.Clone();     } }
    public static ReadOnlyCollection<Type> allCompositeTypes { get { if (!initialized) { Initialize(); } return _allCompositeTypes.AsReadOnly(); } }
    public static ReadOnlyCollection<Type> allDecoratorTypes { get { if (!initialized) { Initialize(); } return _allDecoratorTypes.AsReadOnly(); } }
    public static ReadOnlyCollection<Type> allLeafTypes      { get { if (!initialized) { Initialize(); } return _allLeafTypes.AsReadOnly();      } }

    public static GUIContent[] toolbarComposites { get { if (!initialized) { Initialize(); } return (GUIContent[])_toolbarComposites.Clone(); } }
    public static GUIContent[] toolbarDecorators { get { if (!initialized) { Initialize(); } return (GUIContent[])_toolbarDecorators.Clone(); } }
    public static GUIContent[] toolbarLeaves     { get { if (!initialized) { Initialize(); } return (GUIContent[])_toolbarLeaves.Clone();     } }
    public static ReadOnlyCollection<Type> toolbarCompositeTypes { get { if (!initialized) { Initialize(); } return _toolbarCompositeTypes.AsReadOnly(); } }
    public static ReadOnlyCollection<Type> toolbarDecoratorTypes { get { if (!initialized) { Initialize(); } return _toolbarDecoratorTypes.AsReadOnly(); } }
    public static ReadOnlyCollection<Type> toolbarLeafTypes      { get { if (!initialized) { Initialize(); } return _toolbarLeafTypes.AsReadOnly();      } }

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
                    attributes.Add(new SineInfo(type, ((ShowInNodeEditorAttribute[])attrArray)[0].showInToolbar));
                    Debug.Assert
                    (
                        typeof(BehaviorLeaf).IsAssignableFrom(type) && type != typeof(BehaviorLeaf),
                        "Error: ShowInNodeEditor tags an object that is not derived from BehaviorLeaf"
                    );
                }
            }
        }

        _allComposites = (from attribute in attributes
                          where typeof(BehaviorComposite).IsAssignableFrom(attribute.classType)
                          select new GUIContent(attribute.classType.ToString())).ToArray();

        _toolbarComposites = (from attribute in attributes
                              where typeof(BehaviorComposite).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                              select new GUIContent(attribute.classType.ToString())).ToArray();

        _allCompositeTypes = (from attribute in attributes
                              where typeof(BehaviorComposite).IsAssignableFrom(attribute.classType)
                              select attribute.classType).ToList();

        _toolbarCompositeTypes = (from attribute in attributes
                                  where typeof(BehaviorComposite).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                                  select attribute.classType).ToList();


        _allDecorators = (from attribute in attributes
                          where typeof(BehaviorDecorator).IsAssignableFrom(attribute.classType)
                          select new GUIContent(attribute.classType.ToString())).ToArray();

        _toolbarDecorators = (from attribute in attributes
                              where typeof(BehaviorDecorator).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                              select new GUIContent(attribute.classType.ToString())).ToArray();

        _allDecoratorTypes = (from attribute in attributes
                              where typeof(BehaviorDecorator).IsAssignableFrom(attribute.classType)
                              select attribute.classType).ToList();

        _toolbarDecoratorTypes = (from attribute in attributes
                                  where typeof(BehaviorDecorator).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                                  select attribute.classType).ToList();
        
        _allLeaves = (from attribute in attributes
                          where typeof(BehaviorLeaf).IsAssignableFrom(attribute.classType)
                          select new GUIContent(attribute.classType.ToString())).ToArray();

        _toolbarLeaves = (from attribute in attributes
                          where typeof(BehaviorLeaf).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                          select new GUIContent(attribute.classType.ToString())).ToArray();

        _allLeafTypes = (from attribute in attributes
                         where typeof(BehaviorLeaf).IsAssignableFrom(attribute.classType)
                         select attribute.classType).ToList();

        _toolbarLeafTypes = (from attribute in attributes
                             where typeof(BehaviorLeaf).IsAssignableFrom(attribute.classType) && attribute.showInToolbar
                             select attribute.classType).ToList();
    }

    private static void SetupLists(SineInfo info, ref GUIContent[] allOfNodeType, ref GUIContent[] toolbarOfNodeType, List<Type> allTypeList, List<Type> toolbarTypeList)
    {
        GUIContent[] prevAll = allOfNodeType;
        allOfNodeType = new GUIContent[allOfNodeType.Length + 1];
        for (int i = 0; i < prevAll.Length; i++)
        {
            allOfNodeType[i] = prevAll[i];
        }
        allOfNodeType[prevAll.Length] = new GUIContent(info.classType.ToString());
        allTypeList.Add(info.classType);

        if (info.showInToolbar)
        {
            GUIContent[] prevToolbar = toolbarOfNodeType;
            toolbarOfNodeType = new GUIContent[allOfNodeType.Length + 1];
            for (int i = 0; i < prevToolbar.Length; i++)
            {
                toolbarOfNodeType[i] = prevToolbar[i];
            }
            toolbarOfNodeType[prevToolbar.Length] = new GUIContent(info.classType.ToString());
            toolbarTypeList.Add(info.classType);
        }
    }
}