using UnityEngine;

namespace Benco.Graph
{
    [System.Flags]
    public enum ModifierKeys
    {
        None = 1 << 0,
        Control = 1 << 1,
        Alt = 1 << 2,
        Shift = 1 << 3,
    }

    /// <summary>
    /// Which button is being pressed down. Take the log2() of it to get which button was pressed. 
    /// </summary>
    [System.Flags]
    public enum MouseButtons
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Both = Left | Right,
        Middle = 1 << 2,
    }

    public class UIEvent
    {
        private static readonly EventDelegate emptyDelegate = (Event e) => { };

        /// <summary>
        /// The modifiers the Event should have before the EventHandler is called.
        /// </summary>
        public ModifierKeys modifiers { get; set; }

        /// <summary>
        /// The mouse button(s) the Event must have before the EventHandler is called.
        /// </summary>
        public MouseButtons mouseButtons { get; set; }

        /// <summary>
        /// The EventType the EventHandler must run into before activating.
        /// </summary>
        public EventType eventType { get; set; }

        public bool cancelOnReleaseModifier = false;
        public bool mustHaveAllModifiers = true;
        public bool mustHaveAllMouseButtons = true;

        public bool _cancelOnBothMouseButtonsPressed = true;
        public bool cancelOnBothMouseButtonsPressed
        {
            get { return (mouseButtons & MouseButtons.Both) != MouseButtons.Both && _cancelOnBothMouseButtonsPressed; }
            set { _cancelOnBothMouseButtonsPressed = value; }
        }

        /// <summary>
        /// If eventType is EventType.ExecuteCommand, which command must be called before the EventHandler is called.
        /// </summary>
        public string eventCommand { get; set; }

        public delegate void EventDelegate(Event e);

        /// <summary>
        /// What should be called on the first (and only the first) event. Note that onEventUpdate will still be
        /// called after this. If the event is a Drag action, then this event will use the data of the OnMouseDown
        /// instead.
        /// </summary>
        /// <param name="e">The event that began the EventHandler.</param>
        public EventDelegate onEventBegin = emptyDelegate;

        /// <summary>
        /// Every time the event is updated for KeyDown and Drags, this function will be called.
        /// </summary>
        /// <param name="e">The current event.</param>
        public EventDelegate onEventUpdate = emptyDelegate;

        /// <summary>
        /// The function to be called when the event is completed. Called on the last event frame.
        /// </summary>
        /// <param name="e">The current event.</param>
        public EventDelegate onEventExit = emptyDelegate;

        /// <summary>
        /// The function to be called when the event is canceled. An event can be cancelled via the Escape key, or
        /// if cancelOnDoubleMousePress or cancelOnReleaseModifier is set.
        /// or
        /// </summary>
        /// <param name="e">The current event.</param>
        public EventDelegate onEventCancel = emptyDelegate;

        /// <summary>
        /// The function that gets called when the input requires drawing something to the screen. Used for things like
        /// drawing a selection box, or updating other visuals.
        /// or
        /// </summary>
        /// <param name="e">The current event.</param>
        public EventDelegate onRepaint = emptyDelegate;

        public string name = "";

        public UIEvent(string name = "")
        {
            this.name = name;
        }
    }

}