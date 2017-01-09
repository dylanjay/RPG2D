using UnityEditor;
using UnityEngine;

public class NodeEditorWindow : EditorWindow
{
    public static NodeEditorWindow instance { get; private set; }
    public NodeWorkView workView;
    public NodeToolbarView toolsView;
    public NodeTypeView typeView;
    public NodePropertiesView propertiesView;
    public NodeDescriptionView descriptionView;

    const float viewHorizontalPercentage = 0.75f;
    const float viewVerticalPercentage = 0.7f;
    const float barSize = 0.05f;

    public NodeGraph currentGraph = null;

    public static void Init()
    {
        instance = GetWindow<NodeEditorWindow>();
        instance.InitializeWindow();
        CreateViews();
    }

    public void Update()
    {
        if(currentGraph != null)
        {
            Event e = Event.current;
            currentGraph.UpdateGraph(e);
        }
    }

    public void OnGUI()
    {
        if(workView == null || toolsView == null || typeView == null || propertiesView == null || descriptionView == null)
        {
            CreateViews();
            return;
        }

        Event e = Event.current;

        toolsView.UpdateView(position,
                            new Rect(0, 0, 1, barSize),
                            e, currentGraph);

        typeView.UpdateView(new Rect(position.width, position.height, position.width, position.height),
                            new Rect(0, barSize, barSize, 1),
                            e, currentGraph);

        workView.UpdateView(new Rect(position.width, position.height, position.width, position.height),
                            new Rect(barSize, barSize, viewHorizontalPercentage - barSize, 1),
                            e, currentGraph);

        /*propertiesView.UpdateView(new Rect(position.width, position.height, position.width, position.height),
                            new Rect(viewHorizontalPercentage, barSize, 1 - viewHorizontalPercentage, viewVerticalPercentage),
                            e, currentGraph);

        descriptionView.UpdateView(new Rect(position.width, position.height, position.width, position.height),
                            new Rect(viewHorizontalPercentage, viewVerticalPercentage + barSize, 1 - viewHorizontalPercentage, 1 - viewVerticalPercentage),
                            e, currentGraph);*/

        ProcessEvents(e);
    }

    void InitializeWindow()
    {
        instance.titleContent = new GUIContent("Node Editor");
        instance.maxSize = new Vector2(1000, 600);
        instance.minSize = instance.maxSize;
    }

    static void CreateViews()
    {
        if (instance == null)
        {
            instance = GetWindow<NodeEditorWindow>();
            instance.InitializeWindow();
        }

        else
        {
            instance.workView = new NodeWorkView();
            instance.toolsView = new NodeToolbarView();
            instance.typeView = new NodeTypeView();
            instance.propertiesView = new NodePropertiesView();
            instance.descriptionView = new NodeDescriptionView();
        }
}

    void ProcessEvents(Event e)
    {

    }
}
