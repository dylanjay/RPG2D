using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Benco.Graph;

using Type = System.Type;

namespace Benco.BehaviorTree
{
    public class NodeBehaviorTree : NodeGraph
    {
        public string description = "";

        //TODO(P4): Was this supposed to be private?
        [SerializeField]
        public SharedVariableCollection sharedVariableCollection;

        public override NodeBase root
        {
            get { return base.root; }
            set
            {
                base.root = value;
                if (root != null)
                {
                    root.input.DeleteAllEdges();
                }
            }
        }

        void OnEnable()
        {
            if (sharedVariableCollection == null)
            {
                sharedVariableCollection = new SharedVariableCollection(this);
            }
        }

        public override void UpdateGraph(Event e)
        {
            //If somehow a node becomes null, remove it.
            int removeCount = nodeList.RemoveAll(n => n == null);
            if (removeCount > 0)
            {
                Debug.LogWarning("One of the nodes became null. Removing them from the list.");
                foreach (NodeBase node in nodeList)
                {
                    node.UpdateNode(e);
                }
            }
        }

        public override DeleteReturnType DeleteNode(NodeBase nodeBase)
        {
            BehaviorNodeBase node = nodeBase as BehaviorNodeBase;
            if(nodeList.Contains(node))
            {
                Undo.RecordObject(this, "Deleting Node");
                if (node.behaviorComponent != null)
                {
                    Undo.DestroyObjectImmediate(node.behaviorComponent);
                }
                return base.DeleteNode(nodeBase);
            }
            return DeleteReturnType.DoesNotExist;
        }

        public void AddBehavior(BehaviorNodeBase node)
        {
            sharedVariableCollection.AddBehavior(node);
        }

        public void RemoveBehavior(BehaviorNodeBase node)
        {
            sharedVariableCollection.RemoveBehavior(node);
        }

        public GUIContent[] GetDropdownOptions(Type type)
        {
            return sharedVariableCollection.GetDropdownOptions(type);
        }

        public void SetReference(BehaviorNodeBase node, string fieldName, string prevOption, string currentOption)
        {
            sharedVariableCollection.SetReference(node, fieldName, prevOption, currentOption);
        }

        /// <summary>
        /// Clears a reference that corresponds to the given parameters.
        /// </summary>
        /// <param name="node">The Node that is handling the behavior.</param>
        /// <param name="fieldName">The name of the field being cleared within the behavior.</param>
        /// <param name="sharedVariableName">
        /// The previous option (SharedVariable name) selected in the dropdown menu.
        /// </param>
        public void RemoveReference(BehaviorNodeBase node, string fieldName, string sharedVariableName)
        {
            sharedVariableCollection.RemoveReference(node, fieldName, sharedVariableName);
        }

        public override void OnCreateAsset()
        {
            sharedVariableCollection.OnCreateAsset();
        }

        private static void ShowValueEditor(Type type, object sharedVariable, GUIContent guiContent)
        {
            object value = sharedVariable.GetType().GetField("value").GetValue(sharedVariable);
            System.Func<object, object> func;
            if (_Fields.TryGetValue(type, out func))
            {
                object newValue = func.Invoke(value);

                if (!value.Equals(newValue))
                {
                    sharedVariable.GetType().GetField("value").SetValue(sharedVariable, newValue);
                }
            }
            else if (typeof(Object).IsAssignableFrom(type))
            {
                func = _Fields[typeof(Object)];
                object newValue = func.Invoke(value);

                if (!value.Equals(newValue))
                {
                    sharedVariable.GetType().GetField("value").SetValue(sharedVariable, newValue);
                }
            }
            else
            {
                EditorGUILayout.LabelField("Type " + type + " is unsupported.");
            }
        }

        private static readonly Dictionary<Type, System.Func<object, object>> _Fields =
            new Dictionary<Type, System.Func<object, object>>()
            {
                { typeof(int),      value => EditorGUILayout.DelayedIntField((int)value)},
                { typeof(float),    value => EditorGUILayout.DelayedFloatField((float)value) },
                { typeof(double),   value => EditorGUILayout.DelayedDoubleField((float)value) },
                { typeof(string),   value => EditorGUILayout.DelayedTextField((string)value) },
                { typeof(bool),     value => EditorGUILayout.Toggle((bool)value) },
                { typeof(Vector2),  value => EditorGUILayout.Vector2Field(GUIContent.none, (Vector2)value) },
                { typeof(Vector3),  value => EditorGUILayout.Vector3Field(GUIContent.none, (Vector3)value) },
                { typeof(Vector4),  value => EditorGUILayout.Vector4Field(GUIContent.none, (Vector3)value) },
                { typeof(Bounds),   value => EditorGUILayout.BoundsField((Bounds)value) },
                { typeof(Rect),     value => EditorGUILayout.RectField((Rect)value) },
                { typeof(Object),   value => EditorGUILayout.ObjectField(GUIContent.none, (Object)value, value.GetType(), false)},
            };
    }
}
