using UnityEngine;
using System;

[Serializable]
public class NodeInput
{
    public bool isOccupied = false;
    public NodeBase parentNode;
    public NodeOutput connectedOutput;
    public Vector2 position;
}
