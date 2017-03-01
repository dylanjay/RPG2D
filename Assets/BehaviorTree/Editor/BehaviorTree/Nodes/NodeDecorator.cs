using UnityEngine;
using System.Linq;
using System;
using System.Collections.ObjectModel;

namespace Benco.BehaviorTree.TreeEditor
{
    [Serializable]
    public class NodeDecorator : BehaviorNodeBase
    {
        private static GUIContent[] _allDecoratorOptions = null;
        private static ReadOnlyCollection<Type> _allDecoratorTypes = null;

        public override GUIContent[] GetAllBehaviorOptions()
        {
            if (_allDecoratorOptions == null)
            {
                GUIContent[] tmp = NodeEditorTags.GetAllLabelsOfNodeType(NodeType.Decorator);
                _allDecoratorOptions = new GUIContent[tmp.Length + 1];
                _allDecoratorOptions[0] = new GUIContent("None");
                for (int i = 0; i < tmp.Length; i++)
                {
                    _allDecoratorOptions[i + 1] = tmp[i];
                }
            }
            return _allDecoratorOptions;
        }

        public override ReadOnlyCollection<Type> GetAllBehaviorTypes()
        {
            if (_allDecoratorTypes == null)
            {
                _allDecoratorTypes = NodeEditorTags.GetAllTypesOfNodeType(NodeType.Decorator);
            }
            return _allDecoratorTypes;
        }

        public NodeDecorator(Type nodeType)
        {
            input = new NodeInput();
            output = new NodeOutput();
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
}
