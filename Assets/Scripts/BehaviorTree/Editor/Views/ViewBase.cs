using UnityEngine;
using UnityEditor;
using System;

namespace Benco.Graph
{
    public delegate Rect UpdateRect();

    [Serializable]
    public abstract class ViewBase
    {
        protected NodeEditorWindow parentWindow { get; private set; }

        [SerializeField]
        public Rect displayRect;

        public UpdateRect updateDisplayRect = () => { return Rect.zero; };

        public ViewBase(NodeEditorWindow parentWindow)
        {
            this.parentWindow = parentWindow;
        }

        public abstract void UpdateView(Event e, NodeGraph graph);

        public virtual void ProcessEvents(Event e) { }
    }
}