using UnityEngine;
using UnityEditor;
using System;

namespace Benco.BehaviorTree.TreeEditor
{
    [Serializable]
    public class ViewBase
    {
        public string viewTitle;
        public Rect viewRect;

        protected GUISkin viewSkin;
        protected NodeBehaviorTree currentGraph;

        public ViewBase()
        {
            viewTitle = string.Empty;
            viewRect = new Rect();
            GetEditorSkin();
        }

        public ViewBase(string viewTitle)
        {
            this.viewTitle = viewTitle;
            viewRect = new Rect();
            GetEditorSkin();
        }

        public ViewBase(string viewTitle, Rect viewRect)
        {
            this.viewTitle = viewTitle;
            this.viewRect = viewRect;
            GetEditorSkin();
        }

        public virtual void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeBehaviorTree graph)
        {
            if (viewSkin == null)
            {
                GetEditorSkin();
                return;
            }

            viewRect = new Rect(editorRect.width * percentageRect.x,
                                editorRect.height * percentageRect.y,
                                editorRect.width * percentageRect.width,
                                editorRect.height * percentageRect.height);
            currentGraph = graph;
        }

        public virtual void ProcessEvents(Event e)
        {

        }

        protected void GetEditorSkin()
        {
            if (viewSkin == null)
            {
                viewSkin = UnityEngine.Object.Instantiate(GUI.skin);
            }
            //if (EditorGUIUtility.isProSkin)
            //{
            //    viewSkin = Resources.Load("GUI Skins/Editor/NodeEditorDarkSkin") as GUISkin;
            //}
            //else
            //{
            //    viewSkin = Resources.Load("GUI Skins/Editor/NodeEditorLightSkin") as GUISkin;
            //}
        }
    }
}