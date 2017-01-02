using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to show that this value doesn't matter.
using Wildcard = System.Boolean;
using Type = System.Type;

public class SharedVariables
{
    Dictionary<Type, GUIContent[]> dropdowns = new Dictionary<System.Type, GUIContent[]>();
    Dictionary<GUIContent, List<Tuple<NodeBase, string>>> references = new Dictionary<GUIContent, List<Tuple<NodeBase, string>>>();
    HashSet<string> names = new HashSet<string>();
    Dictionary<Tuple<NodeBase, string>, Wildcard> unassigned = new Dictionary<Tuple<NodeBase, string>, Wildcard>();

    public void AddVariable(string name, Type type)
    {
        if(dropdowns.ContainsKey(type))
        {
            GUIContent[] currentArray = dropdowns[type];
            GUIContent[] newArray = new GUIContent[currentArray.Length + 1];

            for(int i = 0; i < currentArray.Length; i++)
            {
                newArray[i] = currentArray[i];
            }
            newArray[currentArray.Length] = new GUIContent(name);
            dropdowns[type] = newArray;
        }
        else
        {
            GUIContent[] newArray = new GUIContent[1];
            newArray[0] = new GUIContent(name);
            dropdowns[type] = newArray;
        }
    }
}

public class Tuple<T, K>
{
    public T first;
    public K second;
    
    public Tuple(T first, K second)
    {
        this.first = first;
        this.second = second;
    }
}
