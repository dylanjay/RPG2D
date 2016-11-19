using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class NodeBase : ScriptableObject
{
    public string title;
    public string description;

    public Rect nodeRect;
    public NodeGraph parentGraph;
    [HideInInspector]
    public bool isSelected = false;
    [HideInInspector]
    public NodeOutput output;
    [HideInInspector]
    public NodeInput input;
    [HideInInspector]
    public Rect outputRect;
    [HideInInspector]
    public Rect inputRect;
    [HideInInspector]
    public GUISkin nodeSkin;
    Rect viewRect;

    public BehaviorComponent behaviorNode;

    public virtual void Initialize()
    {
        GetEditorSkin();
        if(output != null)
        {
            outputRect = new Rect(nodeRect.x + nodeRect.width / 2 - 8, nodeRect.y + nodeRect.height, 24, 18);
        }

        if(input != null)
        {
            inputRect = new Rect(nodeRect.x + nodeRect.width / 2 - 8, nodeRect.y - 18, 24, 18);
        }
        hideFlags = HideFlags.HideInHierarchy;
    }

    public virtual bool CreateTree()
    {
        return true;
    }

    public virtual void UpdateNode(Event e)
    {
    }
#if UNITY_EDITOR
    public virtual void UpdateNodeGUI(Event e, Rect viewRect)
    {
        this.viewRect = viewRect;
        if(nodeSkin == null)
        {
            GetEditorSkin();
        }
        GUI.Box(nodeRect, name, isSelected ? nodeSkin.GetStyle("NodeSelected") : nodeSkin.GetStyle("NodeDefault"));

        if(output != null)
        {
            outputRect = new Rect(nodeRect.x + nodeRect.width / 2 - 8, nodeRect.y + nodeRect.height, 24, 18);
            GUI.Box(outputRect, "", nodeSkin.GetStyle("NodeConnectionBackBelow"));
            output.position = new Vector2(outputRect.position.x + outputRect.width / 2, outputRect.position.y + outputRect.height / 2);
        }
 
        if (input != null)
        {
            inputRect = new Rect(nodeRect.x + nodeRect.width / 2 - 8, nodeRect.y - 18, 24, 18);
            GUI.Box(inputRect, "", nodeSkin.GetStyle("NodeConnectionBackAbove"));
            input.position = new Vector2(inputRect.position.x + inputRect.width / 2, inputRect.position.y + inputRect.height / 2);
        }

        DrawConnections();
        ProcessEvents(e, viewRect);
        EditorUtility.SetDirty(this);
    }

    void DrawConnections()
    {
        if(input == null) { return; }
        if(input.parentNode == null) { return; }

        Vector3 start = input.connectedOutput.position;
        Vector3 end = input.position;
        Vector2 startTangent = new Vector2();
        Vector2 endTangent = new Vector2();
        Color color = Color.gray;
        if(isSelected || input.parentNode.isSelected)
        {
            color = new Color(1, 0.5f, 0);
        }
        float offset = Mathf.Abs(start.x - end.x) / 1.75f;
        if(input.position.x < start.x)
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

    public virtual void DrawNodeProperties()
    {
        GUILayout.BeginVertical();
        {
            title = EditorGUILayout.TextField("Title", title);
            description = EditorGUILayout.TextField("Description", description);
        }
        GUILayout.EndVertical();
    }

    public virtual void DrawNodeHelp()
    {

    }
#endif

    public virtual void ProcessEvents(Event e, Rect viewRect)
    {
        if(isSelected && e.button == 0)
        {
            if(viewRect.Contains(e.mousePosition))
            {
                if(e.type == EventType.MouseDrag)
                {
                    nodeRect.x += e.delta.x;
                    nodeRect.y += e.delta.y;
                }
            }
        }
        else
        {
            if(viewRect.Contains(e.mousePosition))
            {
                if(e.type == EventType.MouseDrag && e.button == 2)
                {
                    foreach(NodeBase node in parentGraph.nodes)
                    {
                        nodeRect.x += e.delta.x / parentGraph.nodes.Count;
                        nodeRect.y += e.delta.y / parentGraph.nodes.Count;
                    }
                }
            }
        }
    }

    void GetEditorSkin()
    {
        if (EditorGUIUtility.isProSkin)
        {
            nodeSkin = Resources.Load("GUI Skins/Editor/NodeEditorDarkSkin") as GUISkin;
        }
        else
        {
            nodeSkin = Resources.Load("GUI Skins/Editor/NodeEditorLightSkin") as GUISkin;
        }
    }
}
