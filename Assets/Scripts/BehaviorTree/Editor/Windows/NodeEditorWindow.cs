using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Benco.Graph
{
    public class NodeEditorWindow : EditorWindow
    {
        public static NodeEditorWindow instance { get; private set; }

        private static GraphController _graphController;
        public static GraphController graphController
        {
            get
            {
                if (_graphController == null)
                {
                    _graphController = new GraphController();
                }
                return _graphController;
            }
        }

        [SerializeField]
        private List<ViewBase> views = new List<ViewBase>();
        
        public const float TOOLBAR_HEIGHT = 17;
        [SerializeField]
        private float dividerPosition = 200;

        private ViewBase eventReceivingView = null;
        
        [SerializeField]
        public NodeGraph currentGraph = null;

        [MenuItem("Node Editor/Launch Editor")]
        public static void InitNodeEditor()
        {
            instance = GetWindow<NodeEditorWindow>();
            instance.InitializeWindow();
            instance.CreateViews();
        }

        public void Update()
        {
            if (currentGraph != null)
            {
                Event e = Event.current;
                currentGraph.UpdateGraph(e);
            }
        }

        public void OnGUI()
        {
            if (views.Count == 0)
            {
                InitNodeEditor();
                return;
            }
            Event e = Event.current;
            
            if (e.type == EventType.MouseDown)
            {
                for (int i = 0; i < views.Count; i++)
                {
                    if (views[i].displayRect.Contains(e.mousePosition))
                    {
                        eventReceivingView = views[i];
                        break;
                    }
                }
            }
            Event usedEvent = new Event(e);
            if (e.type != EventType.Layout && e.type != EventType.Repaint)
            {
                usedEvent.type = EventType.Used;
            }
            for (int i = 0; i < views.Count; i++)
            {
                views[i].displayRect = views[i].updateDisplayRect();
                views[i].UpdateView(eventReceivingView == views[i] ? e : usedEvent, currentGraph);
            }
        }

        internal void InitializeWindow()
        {
            instance.titleContent = new GUIContent("Node Editor");
        }

        void CreateViews()
        {
            if (instance == null)
            {
                instance = GetWindow<NodeEditorWindow>();
                instance.InitializeWindow();
            }

            NodeWorkView workView = new NodeWorkView();
            NodeToolbarView toolbarView = new NodeToolbarView();
            NodeTypeView typeView = new NodeTypeView();
            toolbarView.updateDisplayRect = () => {
                return new Rect(0, 0, position.width, TOOLBAR_HEIGHT);
            };
            typeView.updateDisplayRect = () => {
                return new Rect(0, TOOLBAR_HEIGHT, dividerPosition,
                                position.height - TOOLBAR_HEIGHT);
            };
            workView.updateDisplayRect = () => {
                return new Rect(dividerPosition,
                                TOOLBAR_HEIGHT,
                                position.width - dividerPosition,
                                position.height - TOOLBAR_HEIGHT);
            };
            views.Add(workView);
            views.Add(toolbarView);
            views.Add(typeView);
        }
    }
}
