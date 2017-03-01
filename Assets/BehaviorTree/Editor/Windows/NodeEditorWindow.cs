using UnityEditor;
using UnityEngine;

namespace Benco.BehaviorTree.TreeEditor
{
    public class NodeEditorWindow : EditorWindow
    {
        public static NodeEditorWindow instance { get; private set; }
        [SerializeField]
        private NodeWorkView workView;

        [SerializeField]
        private NodeToolbarView toolsView;

        [SerializeField]
        private NodeTypeView typeView;
        
        const float viewHorizontalPercentage = 100f;
        const float barSize = 0.05f;

        [SerializeField]
        public NodeBehaviorTree currentGraph = null;

        public static void Init()
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

            

            float heightPercent = (float)GUI.skin.GetStyle("Toolbar").fixedHeight / position.height;

            workView.UpdateView(position,
                                new Rect(0, heightPercent, 1,1),
                                e, currentGraph);

            toolsView.UpdateView(position,
                                new Rect(0, 0, 1, heightPercent),
                                e, currentGraph);

            typeView.UpdateView(position,
                                new Rect(0, barSize, barSize, 1),
                                e, currentGraph);

            ProcessEvents(e);
        }

        void InitializeWindow()
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

        void ProcessEvents(Event e)
        {

        }
    }
}
