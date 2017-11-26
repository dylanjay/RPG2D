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
        private NodeWorkView workView;

        [SerializeField]
        private NodeToolbarView toolsView;

        [SerializeField]
        private NodeTypeView typeView;
        
        public const float TOOLBAR_HEIGHT = 17;
        [SerializeField]
        private float dividerPosition = 200;
        

        [SerializeField]
        public NodeGraph currentGraph = null;

        [MenuItem("Node Editor/Launch Editor")]
        public static void InitNodeEditor()
        {
            instance = GetWindow<NodeEditorWindow>();
            instance.InitializeWindow();
            CreateViews();
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
            if (workView == null || toolsView == null || typeView == null)
            {
                CreateViews();
                return;
            }

            Event e = Event.current;
            
            Rect toolbarViewRect = new Rect(0, 0, position.width, TOOLBAR_HEIGHT);
            Rect typeViewRect = new Rect(0, TOOLBAR_HEIGHT, dividerPosition,
                                         position.height - TOOLBAR_HEIGHT);
            Rect workViewRect = new Rect(dividerPosition, 
                                         TOOLBAR_HEIGHT,
                                         position.width - dividerPosition, 
                                         position.height - TOOLBAR_HEIGHT);

            workView.UpdateView(workViewRect, e, currentGraph);
            typeView.UpdateView(typeViewRect, e, currentGraph);
            toolsView.UpdateView(toolbarViewRect, e, currentGraph);
        }

        internal void InitializeWindow()
        {
            instance.titleContent = new GUIContent("Node Editor");
        }

        static void CreateViews()
        {
            if (instance == null)
            {
                instance = GetWindow<NodeEditorWindow>();
                instance.InitializeWindow();
            }

            instance.workView = new NodeWorkView();
            instance.toolsView = new NodeToolbarView();
            instance.typeView = new NodeTypeView();
        }
    }
}
