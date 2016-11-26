using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class NodeOutput
{
    public List<NodeBase> childNodes = new List<NodeBase>();
    private bool _multipleChildren = true;
    public bool multipleChildren { get { return _multipleChildren; } }
    public Vector2 position;
}
