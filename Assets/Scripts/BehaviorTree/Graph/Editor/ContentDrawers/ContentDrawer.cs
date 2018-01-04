using UnityEngine;

namespace Benco.Graph
{
    /// <summary>
    /// A Strategy class that defines how a given graph element should be drawn onto the screen.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ContentDrawer<T>
    {
        /// <summary>
        /// Defines how important it is to check the Acceptor of this ContentDrawer over others.
        /// ContentDrawers with higher values will be chosen first.
        /// </summary>
        public virtual int priority { get { return 0; } }

        /// <summary>
        /// A function that allows only items with certain properties to be
        /// drawn by this ContentDrawer. If all items of type T should be
        /// drawn with this ContentDrawer, return true.
        /// </summary>
        /// <param name="subject">The item T to potentially draw.</param>
        /// <returns></returns>
        public virtual bool Acceptor(T subject) { return true; }

        /// <summary>
        /// A OnGUI function for the ContentDrawer.
        /// </summary>
        /// <param name="subject">The subject currently being drawn.</param>
        /// <param name="bounds">The bounds calculated from GetBounds.</param>
        /// <param name="e">The event for this OnGUI call.</param>
        public abstract void OnGUI(T subject, DrawingSettings settings, Event e);
    }
}
