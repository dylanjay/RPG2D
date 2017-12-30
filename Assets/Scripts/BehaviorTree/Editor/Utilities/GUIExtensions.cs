// Reflection explained in Seneral's GUIScalUtility: https://gist.github.com/Seneral/2c8e7dfe712b9f53c60f80722fbce5bd
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Benco.Utilities
{
    public static class GUIExtensions
    {
        /// <summary>
        /// A magical value for the rootRect on the clip stack.
        /// </summary>
        public static readonly Rect rootRect = new Rect(-10000, -10000, 40000, 40000);

        private class TrueClipSnapshot
        {
            /// <summary>
            /// The matrix passed in for a given BeginTrueClip call.
            /// </summary>
            public Matrix4x4 matrix { get; private set; }
            /// <summary>
            /// The rectStack that existed before a given BeginTrueClip call.
            /// </summary>
            public Stack<Rect> rectStack { get; private set; }

            public TrueClipSnapshot(Matrix4x4 matrix, Stack<Rect> rectStack)
            {
                this.matrix = matrix;
                this.rectStack = rectStack;
            }
        }
        private static bool initialized = false;

        /// <summary>
        /// Delegate to the getTopRect method pulled by reflection.
        /// </summary>
        private static Func<Rect> getTopRectDelegate;

        /// <summary>
        /// A stack of TrueClipSnapshots.
        /// The inner stack of rects are the rects removed from GUI.Begin/EndClip() stack.
        /// The outer stack of stacks stores the previous removed stack when nesting Begin/EndTrueClip() stacks.
        /// </summary>
        private static Stack<TrueClipSnapshot> snapshotStack = new Stack<TrueClipSnapshot>();


        private static void Initialize()
        {
            Type GUIClip = Assembly.GetAssembly(typeof(GUI)).GetType("UnityEngine.GUIClip", true);
            BindingFlags methodFlags = BindingFlags.Static | BindingFlags.NonPublic;
            MethodInfo getTopRectMethodInfo = GUIClip.GetMethod("GetTopRect", methodFlags);
            
            getTopRectDelegate = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), getTopRectMethodInfo);
            initialized = true;
        }

        /// <summary>
        /// Returns the top rect. Equivalent to calling UnityEngine.GUIClip.GetTopRect().
        /// </summary>
        private static Rect GetTopRect()
        {
            return getTopRectDelegate.Invoke();
        }

        /// <summary>
        /// Begins a clipping region that respects the given matrix and clippingRect.
        /// </summary>
        /// <param name="matrix">The new GUI.matrix value for every direct object in this clipping space.</param>
        /// <param name="clippingRect">The clipping rect with respect to the EditorWindow.</param>
        public static Rect BeginTrueClip(Matrix4x4 matrix, Rect clippingRect)
        {
            if (!initialized) { Initialize(); }
            // Storage for the clippingRects removed to prevent excess clipping.
            Stack<Rect> rectStack = new Stack<Rect>();
            Rect topMostClip = GetTopRect();
            Rect inverseRect = clippingRect.WithOffset(topMostClip.position);
            inverseRect.position = matrix.inverse * new Vector4(inverseRect.position.x, inverseRect.position.y, 0, 1);
            inverseRect.width /= matrix.m00;
            inverseRect.height /= matrix.m11;
            while (topMostClip != rootRect)
            {
                rectStack.Push(topMostClip);
                GUI.EndClip();
                topMostClip = GetTopRect();
            }
            snapshotStack.Push(new TrueClipSnapshot(matrix, rectStack));

            // Begins a clipping space with inverseRect, equivalent to clippingRect's EditorWindow dimensions.
            GUI.BeginClip(inverseRect, -inverseRect.position, Vector2.zero, false);
            return inverseRect;
        }

        /// <summary>
        /// Ends the TrueClip region created in BeginTrueClip.
        /// </summary>
        public static void EndTrueClip()
        {
            // End the special BeginClip() call in BeginTrueClip()
            GUI.EndClip();

            // Call BeginClip on each of the rects originally removed in BeginTrueClip.
            TrueClipSnapshot snapshot;
            try
            {
                snapshot = snapshotStack.Pop();
            }
            catch (InvalidOperationException)
            {
                // Also catches uninitialized errors, because stack is empty then as well.
                Debug.LogError("EndTrueClip() called but does not have corresponding BeginTrueClip()");
                return;
            }
            Stack<Rect> rectStack = snapshot.rectStack;
            while (rectStack.Count > 0)
            {
                GUI.BeginClip(rectStack.Pop());
            }
            GUI.matrix = snapshot.matrix.inverse * GUI.matrix;
        }
    }
}
