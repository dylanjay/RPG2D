﻿using Benco.Utilities;
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

        private float mouseDownPosition = 0;
        private float dividerStartPosition = 0;

        private ViewBase eventReceivingView = null;

        [SerializeField]
        public NodeGraph currentGraph = null;

        UIEventEngine uiEventEngine;

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


            uiEventEngine.OnGUI(e);
            // UnityShenanigans(Ignored):
            // Unity's mouse control for resizing the left panel actually bleeds into the toolbar.
            // I've chosen not to implement this for clarity's sake.
            Rect dividerHitbox = new Rect(dividerPosition - 6, views[2].displayRect.y,
                                          6, views[2].displayRect.height);
            EditorGUIUtility.AddCursorRect(dividerHitbox, MouseCursor.ResizeHorizontal);

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

            uiEventEngine = new UIEventEngine(
                new List<UIEvent>()
                {
                    new UIEvent ("Resize Toolbar Drawer")
                    {
                        mouseButtons = MouseButtons.Left,
                        modifiers = ModifierKeys.All,
                        mustHaveAllModifiers = false,
                        eventType = EventType.MouseDrag,
                        checkedOnEventBegin = OnResizeBegin,
                        onEventUpdate = OnResizeWindow,
                        onRepaint = SetCursorToResizeHorizontal
                    }
                }
            );
        }

        private bool OnResizeBegin(Event e)
        {
            Rect dividerHitbox = new Rect(dividerPosition - 6, views[2].displayRect.y,
                                          6, views[2].displayRect.height);
            mouseDownPosition = e.mousePosition.x;
            dividerStartPosition = dividerPosition;
            return dividerHitbox.Contains(e.mousePosition);
        }

        private void SetCursorToResizeHorizontal(Event e)
        {
            EditorGUIUtility.AddCursorRect(GUIExtensions.rootRect, MouseCursor.ResizeHorizontal);
        }

        private void OnResizeWindow(Event e)
        {
            dividerPosition = dividerStartPosition + (e.mousePosition.x - mouseDownPosition);
            dividerPosition = Mathf.Clamp(dividerPosition, 50, 500);
            Repaint();
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
            toolbarView.updateDisplayRect = () =>
            {
                return new Rect(0, 0, position.width, TOOLBAR_HEIGHT);
            };
            typeView.updateDisplayRect = () =>
            {
                return new Rect(0, TOOLBAR_HEIGHT, dividerPosition,
                                position.height - TOOLBAR_HEIGHT);
            };
            workView.updateDisplayRect = () =>
            {
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
