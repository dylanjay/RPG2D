namespace Benco.Graph
{
    /// <summary>
    /// An adapter that only exposes read permissions of GraphEditorSettings.
    /// </summary>
    public struct DrawingSettings
    {
        public bool snapToGrid /*    */ { get { return graphEditorSettings.snapToGrid; } }
        public bool snapDimensions /**/ { get { return graphEditorSettings.snapDimensions; } }
        public int snapSize /*       */ { get { return graphEditorSettings.snapSize; } }
        public bool showHotbar /*    */ { get { return graphEditorSettings.showHotbar; } }
        public bool showAxes /*      */ { get { return graphEditorSettings.showAxes; } }

        private GraphEditorSettings graphEditorSettings;

        public DrawingSettings(GraphEditorSettings graphEditorSettings)
        {
            this.graphEditorSettings = graphEditorSettings;
        }
    }
}