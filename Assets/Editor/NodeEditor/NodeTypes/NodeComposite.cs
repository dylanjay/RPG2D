using UnityEngine;
using System.Linq;
using System;
using System.Collections.ObjectModel;

[Serializable]
public class NodeComposite : NodeBase
{
    private static GUIContent[] _allCompositeOptions = null;
    private static ReadOnlyCollection<Type> _allCompositeTypes = null;

    public override GUIContent[] GetAllBehaviorOptions()
    {
        if (_allCompositeOptions == null)
        {
            GUIContent[] tmp = NodeEditorTags.GetAllLabelsOfNodeType(NodeType.Composite);
            _allCompositeOptions = new GUIContent[tmp.Length + 1];
            _allCompositeOptions[0] = new GUIContent("None");
            for (int i = 0; i < tmp.Length; i++)
            {
                _allCompositeOptions[i + 1] = tmp[i];
            }
        }
        return _allCompositeOptions;
    }

    public override ReadOnlyCollection<Type> GetAllBehaviorTypes()
    {
        if (_allCompositeTypes == null)
        {
            _allCompositeTypes = NodeEditorTags.GetAllTypesOfNodeType(NodeType.Composite);
        }
        return _allCompositeTypes;
    }

    public override void Initialize()
    {
        base.Initialize();
        nodeRect = new Rect(10, 10, 150, 35);
    }

    public override void UpdateNodeGUI(Event e, Rect viewRect)
    {
        base.UpdateNodeGUI(e, viewRect);
    }
#if UNITY_EDITOR
    public override void DrawNodeProperties()
    {
        base.DrawNodeProperties();
    }

    public override void DrawNodeHelp()
    {
        base.DrawNodeHelp();
    }
#endif
}
