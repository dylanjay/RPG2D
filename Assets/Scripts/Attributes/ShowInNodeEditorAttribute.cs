using System;

namespace Benco.Graph
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ShowInNodeEditorAttribute : Attribute
    {
        /// <summary>
        /// The name that should be displayed in the graph editor. Overrides NodeTypeAttribute's displayName.
        /// </summary>
        public string displayName = "";

        private string _menuPath = "";
        /// <summary>
        /// The path in the node creation menu. This value can combine with a NodeType attribute if this value uses 
        /// "{0}" to denote the NodeTypeAttribute's menuPath value (e.g. if NodeType.menuPath = "Composite", and 
        /// ShowInNodeEditor.menuPath = "{0}/Memory/Selector", then the path = "Composite/Memory/Selector"). 
        /// 
        /// Note that if the class tagged with this attribute is abstract, a "/" will be inserted after the menuPath
        /// string automatically if no class uses it's menuPath. Otherwise, if the class is concrete and ends with a 
        /// "/", the display name will be appended to the end to allow for the node to be instantiated.
        /// </summary>
        public string menuPath
        {
            get
            {
                if (_menuPath != "")
                {
                    return _menuPath;
                }
                else
                {
                    return string.Format("{{0}}/{0}", displayName);
                }
            }
        }

        public ShowInNodeEditorAttribute()
        {
        }

        public ShowInNodeEditorAttribute(string displayName)
        {
            this.displayName = displayName;
            _menuPath = string.Format("{{0}}/{0}", displayName);
        }

        public ShowInNodeEditorAttribute(string displayName, string menuPath)
        {
            this._menuPath = menuPath;
            this.displayName = displayName;
        }
    }

    /// <summary>
    /// An attribute that will tag a class that implements INode with the correct nodes to create within the Node 
    /// Editor Window. Often used with abstract base classes to set the node type of all inheriting classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeTypeAttribute : Attribute
    {
        /// <summary>
        /// The type of node the class should be associated with.
        /// </summary>
        public readonly Type nodeType;
        /// <summary>
        /// The name that should be displayed in the graph editor. This value will be ignored if a ShowInNodeEditor 
        /// attribute exists on the same object.
        /// </summary>
        public readonly string displayName = "";
        
        private string _menuPath = "";
        /// <summary>
        /// The path in the node creation menu. This value can combine with a ShowInNodeEditor attribute if the 
        /// ShowInNodeEditor attribute uses "{0}" to denote this NodeTypeAttribute's menuPath value (e.g. if 
        /// NodeType.menuPath = "Composite", and ShowInNodeEditor.menuPath = "{0}/Memory/Selector", then the path = 
        /// "Composite/Memory/Selector"). 
        /// 
        /// Note that if the class tagged with this attribute is abstract, a "/" will be inserted after the menuPath
        /// string automatically if no class uses it's menuPath. Otherwise, if the class is concrete and ends with a 
        /// "/", the display name will be appended to the end to allow for the node to be instantiated.
        /// </summary>
        public string menuPath
        {
            get
            {
                if (_menuPath != "")
                {
                    return _menuPath;
                }
                else
                {
                    return string.Format("{{0}}/{0}", displayName);
                }
            }
        }

        /// <param name="nodeType">The type of node the class should be associated with.</param>
        public NodeTypeAttribute(Type nodeType)
        {
            this.nodeType = nodeType;
        }

        public NodeTypeAttribute(Type nodeType, string displayName)
        {
            this.nodeType = nodeType;
            this.displayName = displayName;
            _menuPath = string.Format("{{0}}/{0}", displayName);
        }

        public NodeTypeAttribute(Type nodeType, string displayName, string menuPath)
        {
            this.nodeType = nodeType;
            this.displayName = displayName;
            _menuPath = menuPath;
        }
    }
}
