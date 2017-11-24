using UnityEngine;
using UnityEditor;
using System;

namespace Benco.Graph
{
    [Serializable]
    public class ViewBase
    {
        public Rect viewRect;
        
        public ViewBase()
        {
            viewRect = new Rect();
        }

        public ViewBase(Rect viewRect)
        {
            this.viewRect = viewRect;
        }

        protected virtual void UpdateView(Rect editorRect, Rect percentageRect, Event e)
        {
            viewRect = new Rect(editorRect.width * percentageRect.x,
                                editorRect.height * percentageRect.y,
                                editorRect.width * percentageRect.width,
                                editorRect.height * percentageRect.height);
        }

        public virtual void ProcessEvents(Event e)
        {

        }
    }
}