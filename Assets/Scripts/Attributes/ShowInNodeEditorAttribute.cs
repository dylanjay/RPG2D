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
