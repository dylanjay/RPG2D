using UnityEngine;
using System;

[Serializable]
public class NodeInput
{
    public bool isOccupied = false;
    public NodeBase parentNode;
    public Vector2 position;
}
