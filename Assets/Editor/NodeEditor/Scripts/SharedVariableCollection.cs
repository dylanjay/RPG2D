using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

using Type = System.Type;
using Attribute = System.Attribute;
using Wildcard = System.SByte;

[System.Serializable]
public class SharedVariableCollection
{
    [System.Serializable]
    public struct NodeFieldPair
    {
        public NodeBase node;
        public string fieldName;

        public NodeFieldPair(NodeBase node, string fieldName)
        {
            this.node = node;
            this.fieldName = fieldName;
        }
    }

    //This pattern is the only way to properly serialize these variables, since Unity/C#
    //cannot serialize Generic objects by default.
    [System.Serializable]
    protected class DropdownDictionary : SerializableDictionary<Type, List<GUIContent>> { }
    /// <summary>
    /// All possible dropdowns. The key is the type of the object, the value is the list of GUIContent options.
    /// </summary>
    [SerializeField]
    protected DropdownDictionary dropdowns = new DropdownDictionary();

    [System.Serializable]
    protected class ReferenceDictionary : SerializableDictionary<GUIContent, HashSet<NodeFieldPair>> { }
    /// <summary>
    /// A dictionary of all references found within the tree. The key is the GUIContent option from the dropdown,
    /// The value is a hashset of Node/FieldName pairs that correspond to that variable.
    /// </summary>
    [SerializeField]
    protected ReferenceDictionary references = new ReferenceDictionary();

    [System.Serializable]
    protected class ValuesDictionary : SerializableDictionary<GUIContent, object> { }
    /// <summary>
    /// A dictinary that holds all SharedVariable values. The key is the GUIContent option, the value is the object
    /// stored for that SharedVariable name.
    /// </summary>
    [SerializeField]
    protected ValuesDictionary values = new ValuesDictionary();

    protected class NameHashSet : SerializableDictionary<string, Wildcard>
    {
        public void Add(string name) { Add(name, default(Wildcard)); }
    }
    /// <summary>
    /// A HashSet of all strings that are being used for a SharedVariable name in the corresponding tree.
    /// </summary>
    [SerializeField]
    protected NameHashSet names = new NameHashSet();

    protected static GUIContent none = new GUIContent("None");

    protected HashSet<NodeFieldPair> unassigned
    {
        get { return references[none]; }
        set { references[none] = value; }
    }

    public SharedVariableCollection()
    {
        references.Add(none, new HashSet<NodeFieldPair>());
        values.Add(none, null);
        names.Add(none.text);
    }

    /// <summary>
    /// Adds a new SharedVariable to the tracked collection of shared variables.
    /// </summary>
    /// <param name="name">The name of the new variable.</param>
    /// <param name="type">The type of the new variable.</param>
    public void AddVariable(string name, Type type)
    {
        if (names.ContainsKey(name))
        {
            Debug.LogError("Error: a SharedVariable already exists in this tree with the given name.");
            return;
        }
        if (name.Length == 0)
        {
            Debug.LogError("Error: name must be at least one character long.");
            return;
        }

        names.Add(name);

        if (dropdowns.ContainsKey(type))
        {
            //Add the name to the given type.
            dropdowns[type].Add(new GUIContent(name));
        }
        else
        {
            //Create the GUIContent list for the given type.
            dropdowns[type] = new List<GUIContent>() { new GUIContent(name) };
        }

        references.Add(dropdowns[type][dropdowns[type].Count - 1], new HashSet<NodeFieldPair>());
        foreach(Type sharedVarType in NodeUtilities.GetSharedVariableDerivedTypes())
        {
            if(sharedVarType.BaseType.GetGenericArguments()[0] == type)
            {
                values.Add(dropdowns[type][dropdowns[type].Count - 1], System.Activator.CreateInstance(sharedVarType, true));
                return;
            }
        }
        Debug.LogError("Error: recieved a type that does not have an associated SharedVariable<> derived class. Type: " + type);
    }

    /// <summary>
    /// Removes a variable from the collection of shared variables.
    /// Prints out an error upon failure.
    /// </summary>
    /// <param name="name">The name of the variable we are trying to remove.</param>
    /// <param name="type"></param>
    public void RemoveVariable(string name, Type type)
    {
        if (!dropdowns.ContainsKey(type))
        {
            Debug.LogError("Error: tried to remove a value from a shared variable type that does not exist.");
        }
        List<GUIContent> guiContents = dropdowns[type];
        GUIContent match = guiContents.Find(x => x.text == name);
        if(match == default(GUIContent))
        {
            Debug.LogError("Error: tried to remove a value that does not exist.");
            return;
        }

        if(references[match].Count != 0)
        {
            Debug.LogWarning(string.Format("There nodes that are still currently using SharedVariable \"{0}\". These references will be cleared."));
            foreach(NodeFieldPair reference in references[match])
            {
                typeof(NodeBase).GetField(reference.fieldName).SetValue(reference.node.behaviorComponent, null);
                unassigned.Add(reference);
            }
            references.Remove(match);
        }
        
        if (guiContents.Count == 1)
        {
            dropdowns.Remove(type);
        }
        else
        {
            dropdowns[type].Remove(match);
        }
        
        names.Remove(name);
        values.Remove(match);
    }

    /// <summary>
    /// Renames a variable, keeping the same type it had before.
    /// </summary>
    /// <param name="oldName">The old name.</param>
    /// <param name="newName">The new name.</param>
    /// <param name="type">The type the variable is.</param>
    /// <remarks>
    /// It's important to note that we don't want to remove the value and add it back in.
    /// If we did this, the nodes would lose their references to the SharedVariables, and
    /// all of them would have to be reset.
    /// </remarks>
    public void RenameVariable(string oldName, string newName, Type type)
    {
        if (names.ContainsKey(newName))
        {
            Debug.LogError("Error: tried to rename variable to the same name as another variable.");
            return;
        }
        if (!dropdowns.ContainsKey(type))
        {
            Debug.LogError("Error: tried to rename a value from a shared variable type that does not exist.");
            return;
        }
        List<GUIContent> guiContents = dropdowns[type];
        for (int i = 0; i < guiContents.Count; i++)
        {
            if (guiContents[i].text == oldName)
            {
                guiContents[i].text = newName;
                names.Remove(oldName);
                names.Add(newName);
                return;
            }
        }
        //If we did not find and rename the value.
        Debug.LogError(string.Format("Error: could not find variable {0} of type {1} to rename.", oldName, type));
    }

    /// <summary>
    /// Changes the type of the SharedVariable from <paramref name="oldType"/> to <paramref name="newType"/>.
    /// </summary>
    /// <param name="oldType"></param>
    /// <param name="newType"></param>
    /// <param name="variableName"></param>
    /// <remarks>
    /// Because the type is changing, and the SharedVariable requires a specific type, we should clear out the references
    /// in the nodes that contain this SharedVariable.
    /// </remarks>
    public void ChangeVariableType(Type oldType, Type newType, string variableName)
    {
        RemoveVariable(variableName, oldType);
        AddVariable(variableName, newType);
    }

    /// <summary>
    /// Gets a GUIContent[] populated with the names of each variable for that given type. Useful for calling EditorGUI.Popup.
    /// </summary>
    /// <param name="type">The type </param>
    /// <returns>All SharedVariable names with the given type in the form of a GUIContent[].</returns>
    public GUIContent[] GetDropdownOptions(Type type)
    {
        if(dropdowns.ContainsKey(type))
        {
            List<GUIContent> list = new List<GUIContent> { none };
            list.AddRange(dropdowns[type]);
            return list.ToArray();
        }
        else
        {
            //Debug.LogWarning(string.Format("Warning: No shared variables of type {0}.", type));
            return new GUIContent[1]{none};
        }
    }

    /// <summary>
    /// Sets a SharedVariable to the given variable associated with the GUIContent.
    /// </summary>
    /// <param name="node">The Node that is handling the behavior.</param>
    /// <param name="fieldName">The name of the field being set within the behavior.</param>
    /// <param name="option">The option selected in the dropdown menu. Must not be a copy.</param>
    public void SetReference(NodeBase node, string fieldName, GUIContent prevOption, GUIContent currentOption)
    {
        NodeFieldPair sharedVarPair = new NodeFieldPair(node, fieldName);

        references[prevOption].Remove(sharedVarPair);
        references[currentOption].Add(sharedVarPair);

        Type nodeType = node.behaviorComponent.GetType();
        FieldInfo fieldInfo =
        nodeType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        fieldInfo.SetValue(node.behaviorComponent, values[currentOption]);
    }

    /// <summary>
    /// Clears a reference that corresponds to the given parameters.
    /// </summary>
    /// <param name="node">The Node that is handling the behavior.</param>
    /// <param name="fieldName">The name of the field being cleared within the behavior.</param>
    /// <param name="option">The previous option selected in the dropdown menu. Must not be a copy.</param>
    public void RemoveReference(NodeBase node, string fieldName, GUIContent option)
    {
        NodeFieldPair sharedVarPair = new NodeFieldPair(node, fieldName);
        references[option].Remove(sharedVarPair);
        unassigned.Add(sharedVarPair);

        Type nodeType = node.behaviorComponent.GetType();
        nodeType.GetField(fieldName).SetValue(node.behaviorComponent, null);
    }

    /// <summary>
    /// Call this funciton when a node is added to the behavior tree.
    /// </summary>
    /// <param name="node">The new node.</param>
    public void AddBehavior(NodeBase node)
    {
        Type behaviorType = node.behaviorComponent.GetType();

        foreach (FieldInfo fieldInfo in behaviorType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            //If it is a shared variable and a SerializedField or isPublic
            if (fieldInfo.FieldType.IsSubClassOfGeneric(typeof(SharedVariable<>)) &&
                (((Attribute[])fieldInfo.GetCustomAttributes(typeof(SerializeField), true)).Length > 0 || fieldInfo.IsPublic))
            {
                unassigned.Add(new NodeFieldPair(node, fieldInfo.Name));
            }
        }
    }

    /// <summary>
    /// Call this function when a node is removed from the behavior tree.
    /// </summary>
    /// <param name="node">The node removed.</param>
    public void RemoveBehavior(NodeBase node)
    {
        Type behaviorType = node.behaviorComponent.GetType();

        foreach(FieldInfo fieldInfo in behaviorType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            //If it is a shared variable and a SerializedField or isPublic
            if(fieldInfo.FieldType.IsSubClassOfGeneric(typeof(SharedVariable<>)) &&
                (((Attribute[])fieldInfo.GetCustomAttributes(typeof(SerializeField), true)).Length > 0 && fieldInfo.IsPrivate ||
                (((Attribute[])fieldInfo.GetCustomAttributes(typeof(HideInInspector), true)).Length != 0 && fieldInfo.IsPublic)))
            {
                object sharedVar = (fieldInfo.GetValue(node.behaviorComponent));
                if (sharedVar == null)
                {
                    unassigned.Remove(new NodeFieldPair(node, fieldInfo.Name));
                }
                else
                {
                    Type varType = (Type)sharedVar.GetType().GetProperty("sharedType").GetGetMethod().Invoke(sharedVar, new object[] { });

                    GUIContent guiContent = dropdowns[varType].ToList().Find(x => x.text == fieldInfo.Name);
                    references[guiContent].Remove(new NodeFieldPair(node, fieldInfo.Name));
                }
            }
        }
    }

    /// <summary>
    /// Call this function when the behavior within the node changes.
    /// </summary>
    /// <param name="node">The modified node.</param>
    public void NodeBehaviorChanged(NodeBase node)
    {
        RemoveBehavior(node);
        AddBehavior(node);
    }

    /// <summary>
    /// Sets the value of the given SharedVariable option to the given value. Uses reflection.
    /// </summary>
    /// <param name="namedOption">The option in the Popup dropdown menu.</param>
    /// <param name="value">The new value the SharedVariable should hold.</param>
    public void SetValue(GUIContent namedOption, object value)
    {
        values[namedOption].GetType().GetField("value").SetValue(values[namedOption], value);
    }
    
    public IEnumerator<NodeFieldPair> GetAllUnAssigned()
    {
        HashSet<NodeFieldPair>.Enumerator enumerator = unassigned.GetEnumerator();

        while(enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    public HashSet<NodeFieldPair>.Enumerator GetUnAssignedEnumerator()
    {
        return unassigned.GetEnumerator();
    }

    public IDictionary<GUIContent, object> GetValues()
    {
        return values;
    }

    public void FinishSetup()
    {
        if (unassigned.Any())
        {
            Debug.LogError("Error: Some nodes still have unassigned references. " +
                "If this is intentional, set your variable as private/protected without a SerializedField attribute, " +
                "or as public/internal with a HideInInspector attribute.");
            return;
        }

        foreach(GUIContent guiContent in values.Keys)
        {
            values[guiContent].GetType().GetField("name").SetValue(values[guiContent], guiContent.text);
        }
    }
}