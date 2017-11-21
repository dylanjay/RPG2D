using UnityEngine;
using UnityEditor;
using System;

namespace Benco.Graph
{
    [Serializable]
    public class ViewBase
    {
        public string viewTitle;
        public Rect viewRect;
        
        public ViewBase()
        {
            viewTitle = string.Empty;
            viewRect = new Rect();
        }

        public ViewBase(string viewTitle)
        {
            this.viewTitle = viewTitle;
            viewRect = new Rect();
        }

        public ViewBase(string viewTitle, Rect viewRect)
        {
            this.viewTitle = viewTitle;
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