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
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, 
                                                             GUIStyle.none, 
                                                             GUI.skin.verticalScrollbar);
            
            EditorGUI.BeginChangeCheck();
            GUIContent showAxes = new GUIContent("Show Axes", "Adds bold lines where the origin is located.");
            settings.showAxes = EditorGUILayout.Toggle(showAxes, settings.showAxes);

            GUIContent showHotbar = new GUIContent("Show Hot Bar", 
                                                   "Toggles the display of the hotbar window on the left.");
            settings.showHotbar = EditorGUILayout.Toggle(showHotbar, settings.showHotbar);

            GUIContent snapToGrid = new GUIContent("Snap To Grid", 
                                                   "When moving nodes, whether or not to snap the nodes into place.");
            settings.snapToGrid = EditorGUILayout.Toggle(snapToGrid, settings.snapToGrid);

            EditorGUI.BeginDisabledGroup(!settings.snapToGrid);
            {
                int snapSize = settings.snapSize;
                GUIContent snapGuiContent = new GUIContent("Snap Size",
                                                           "At what intervals should the grid snap to?");
                snapSize = EditorGUILayout.DelayedIntField(snapGuiContent, snapSize);
                settings.snapSize = Mathf.Clamp(snapSize, 1, 100);

                GUIContent snapDimensions = new GUIContent("Dimensions Match Grid",
                                                           "Should the dimensions of the node line up with the grid?");
                settings.snapDimensions = EditorGUILayout.Toggle(snapDimensions, settings.snapDimensions);
            }
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                graphViewer.Repaint();
                settings.SaveSettings();
            }


            EditorGUI.BeginDisabledGroup(true);
            GUIContent zoom = new GUIContent("Zoom",
                                             "The current scale of the camera. This can be modified by scrolling " +
                                             "with the mouseWheel. To jump to the nearest power of 2 zoom, hold " +
                                             "CTRL/CMD and scroll.");
            EditorGUILayout.FloatField(zoom, graphViewer.scale.x);
            EditorGUI.EndDisabledGroup();

            bool prevShowOriginValue = graphViewer.scale == Vector2.one && graphViewer.offset == Vector2.zero;
            GUIContent originContent = new GUIContent("Show Origin",
                                                      "If enabled, moves the camera back to the original position. " +
                                                      "Can be redisabled to jump back to the previous position.");
            bool showOrigin = EditorGUILayout.Toggle(originContent, prevShowOriginValue);
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
