﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NodeGraph : ScriptableObject
{
    public string graphName = "New Graph";
    public List<NodeBase> nodes;
    [HideInInspector]
    public NodeBase selectedNode;
    [HideInInspector]
    public bool wantsConnection;
    [HideInInspector]
    public NodeBase connectionNode;
    private NodeBase disconnectNode;
    [HideInInspector]
    public bool showProperties;
    public NodeBase rootNode;

    void OnEnable()
    {
        if(nodes == null)
        {
            nodes = new List<NodeBase>();
        }
    }

    public void Initialize()
    {
        if (nodes.Any())
        {
            foreach(NodeBase node in nodes)
            {
                node.Initialize();
            }
        }
    }

    public void UpdateGraph(Event e)
    {
        if (selectedNode == null)
        {
            showProperties = false;
        }

        else
        {
            showProperties = true;
        }

        if (nodes.Any())
        {
            foreach(NodeBase node in nodes)
            {
                node.UpdateNode(e);
            }
        }
    }

#if UNITY_EDITOR
    public void UpdateGraphGUI(Event e, Rect viewRect)
    {
        if(nodes.Any())
        {
            ProcessEvents(e, viewRect);
            foreach(NodeBase node in nodes)
            {
                node.UpdateNodeGUI(e, viewRect);
            }
        }

        if(wantsConnection)
        {
            if(connectionNode != null)
            {
                DrawConnectionToMouse(e.mousePosition);
            }
        }
    }

    void ProcessEvents(Event e, Rect viewRect)
    {
        if (viewRect.Contains(e.mousePosition))
        {
            if (e.button == 0)
            {
                if (e.type == EventType.MouseDown)
                {
                    DeselectAllNodes();
                    selectedNode = null;
                    foreach (NodeBase node in nodes)
                    {
                        if(node.nodeRect.Contains(e.mousePosition))
                        {
                            selectedNode = node;
                            node.isSelected = true;
                            break;
                        }

                        else if(node.output != null)
                        {
                            if (node.outputRect.Contains(e.mousePosition))
                            {
                                connectionNode = node;
                                wantsConnection = true;
                            }

                            foreach(NodeBase childNode in node.output.childNodes)
                            {
                                if (childNode.inputRect.Contains(e.mousePosition))
                                {
                                    connectionNode = node;
                                    disconnectNode = childNode;
                                    wantsConnection = true;
                                }
                            }
                        }
                    }
                }

                else if(e.type == EventType.MouseUp)
                {
                    bool hitNode = false;
                    if (wantsConnection)
                    {
                        foreach (NodeBase node in nodes)
                        {
                            if (node.input != null)
                            {
                                if (node.inputRect.Contains(e.mousePosition))
                                {
                                    hitNode = true;
                                    if (node != connectionNode)
                                    {
                                        DeselectAllNodes();
                                        selectedNode = null;

                                        if (disconnectNode != null)
                                        {
                                            DisconnectNodes(connectionNode, disconnectNode);
                                        }

                                        if (connectionNode.GetType() == typeof(NodeDecorator) && connectionNode.output.childNodes.Any())
                                        {
                                            DisconnectNodes(connectionNode, connectionNode.output.childNodes[0]);
                                        }

                                        node.input.parentNode = connectionNode;
                                        node.input.isOccupied = node.input.parentNode != null;
                                        connectionNode.output.childNodes.Add(node);
                                        node.input.connectedOutput = connectionNode.output;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!hitNode && disconnectNode != null)
                        {
                            DisconnectNodes(connectionNode, disconnectNode);
                        }
                    }
                    wantsConnection = false;
                    connectionNode = null;
                    disconnectNode = null;
                }
            }
        }

        if(e.keyCode == KeyCode.Delete && selectedNode != null)
        {
            DeleteNode(selectedNode);
            selectedNode = null;
        }
    }
    
    void DisconnectNodes(NodeBase male, NodeBase female)
    {
        male.output.childNodes.Remove(female);
        female.input.connectedOutput = null;
        female.input.parentNode = null;
        female.input.isOccupied = false;
    }

    void DrawConnectionToMouse(Vector2 position)
    {
        Vector3 start = connectionNode.output.position;
        Vector3 end = position;
        Vector2 startTangent = new Vector2();
        Vector2 endTangent = new Vector2();
        Color color = Color.gray;
        color = new Color(1, 0.5f, 0);
        float offset = Mathf.Abs(start.x - end.x) / 1.75f;
        if (position.x < start.x)
        {
            startTangent = new Vector2(start.x - offset, start.y);
            endTangent = new Vector2(end.x + offset, end.y);
        }
        else
        {
            startTangent = new Vector2(start.x + offset, start.y);
            endTangent = new Vector2(end.x - offset, end.y);
        }

        Handles.BeginGUI();
        {
            Handles.color = Color.white;
            Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 2);
        }
        Handles.EndGUI();
    }

    public void DeleteNode(NodeBase node)
    {
        if(node.output != null)
        {
            foreach(NodeBase child in node.output.childNodes)
            {
                if(child.input.parentNode == node)
                {
                    child.input.connectedOutput = null;
                    child.input.parentNode = null;
                    child.input.isOccupied = false;
                }
            }
        }
        nodes.Remove(node);
    }
#endif

    bool isConnected(NodeBase first, NodeBase second)
    {
        if (first.output != null)
        {
            foreach (NodeBase node in first.output.childNodes)
            {
                if (node == second)
                {
                    return true;
                }
            }
        }

        if (second.output != null)
        {
            foreach (NodeBase node in second.output.childNodes)
            {
                if (node == first)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void DeselectAllNodes()
    {
        foreach(NodeBase node in nodes)
        {
            node.isSelected = false;
        }
        selectedNode = null;
    }
}
