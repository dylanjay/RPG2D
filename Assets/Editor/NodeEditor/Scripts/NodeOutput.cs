﻿using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class NodeOutput
{
    public List<NodeBase> childNodes = new List<NodeBase>();
    //public int connectedInputID = -1;
    public Vector2 position;
}
