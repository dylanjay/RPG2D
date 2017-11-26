using UnityEngine;
using UnityEditor;
using System;

namespace Benco.Graph
{
    [Serializable]
    public abstract class ViewBase
    {
        public ViewBase() { }

        public abstract void UpdateView(Rect displayRect, Event e, NodeGraph graph);

        public virtual void ProcessEvents(Event e) {}
    }
}