using System;

namespace Benco.BehaviorTree
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ShowInNodeEditorAttribute : Attribute
    {
        public readonly bool showInToolbar;
        public readonly string displayName;
        public readonly bool generic;

        public ShowInNodeEditorAttribute(string displayName, bool showInToolbar)
        {
            this.showInToolbar = showInToolbar;
            this.displayName = displayName;
        }
    }

    public enum NodeType { Composite, Decorator, Leaf }
}