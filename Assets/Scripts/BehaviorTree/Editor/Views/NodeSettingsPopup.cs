using UnityEditor;
using UnityEngine;

namespace Benco.Graph
{
    public class NodeSettingsPopup : PopupWindowContent
    {
        private GraphEditorSettings settings;
        private GraphViewer graphViewer;
        private Vector2 lastOffset = Vector2.one;
        private Vector2 lastScale = Vector2.one;

        public NodeSettingsPopup (GraphEditorSettings settings, GraphViewer graphViewer)
        {
            this.settings = settings;
            this.graphViewer = graphViewer;
        }
        private Vector2 scrollPosition = Vector2.zero;
        
        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);


            EditorGUI.BeginChangeCheck();
            settings.showAxes = EditorGUILayout.Toggle("Show Axes", settings.showAxes);
            settings.showHotbar = EditorGUILayout.Toggle("Show Hot Bar", settings.showHotbar);
            settings.snapToGrid = EditorGUILayout.Toggle("Snap To Grid", settings.snapToGrid);

            EditorGUI.BeginDisabledGroup(!settings.snapToGrid);
            {
                int snapSize = settings.snapSize;
                snapSize = EditorGUILayout.DelayedIntField("Snap Size", snapSize);
                settings.snapSize = Mathf.Clamp(snapSize, 1, 100);
                
                settings.snapDimensions = EditorGUILayout.Toggle("Dimensions Match Grid", settings.snapDimensions);
            }
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                settings.SaveSettings();
            }


            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField("Zoom", graphViewer.scale.x);
            EditorGUI.EndDisabledGroup();

            bool prevShowOriginValue = graphViewer.scale == Vector2.one && graphViewer.offset == Vector2.zero;
            bool showOrigin = EditorGUILayout.Toggle("Show Origin", prevShowOriginValue);
            if (showOrigin != prevShowOriginValue)
            {
                if (showOrigin)
                {
                    lastScale = graphViewer.scale;
                    lastOffset = graphViewer.offset;
                    graphViewer.scale = Vector2.one;
                    graphViewer.offset = Vector2.zero;
                }
                else
                {
                    graphViewer.scale = lastScale;
                    graphViewer.offset = lastOffset;
                }
                graphViewer.Repaint();
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
