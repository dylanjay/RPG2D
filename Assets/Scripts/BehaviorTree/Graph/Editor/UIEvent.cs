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
        private static readonly CheckedEventDelegate emptyCheckedDelegate = (Event e) => { return true; };

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
        /// If eventType is EventType.ExecuteCommand, which command must be called before the
        /// EventHandler is called.
        /// </summary>
        public string eventCommand { get; set; }

        public delegate void EventDelegate(Event e);
        public delegate bool CheckedEventDelegate(Event e);

        /// <summary>
        /// A version of <see cref="onEventBegin"/> that cancels the event upon returning false.
        /// </summary>
        public CheckedEventDelegate checkedOnEventBegin = emptyCheckedDelegate;
        /// <summary>
        /// What should be called on the first (and only the first) event. Note that onEventUpdate
        /// will still be called after this. If the event is a Drag action, then this event will
        /// use the data of the OnMouseDown instead.
        /// </summary>
        public EventDelegate onEventBegin { set { checkedOnEventBegin = (Event e) => { value(e); return true; }; } }

        /// <summary>
        /// A version of <see cref="onEventUpdate"/> that cancels the event upon returning false.
        /// </summary>
        public CheckedEventDelegate checkedOnEventUpdate = emptyCheckedDelegate;
        /// <summary>
        /// Every time the event is updated for KeyDown and Drags, this function will be called.
        /// </summary>
        public EventDelegate onEventUpdate { set { checkedOnEventUpdate = (Event e) => { value(e); return true; }; } }
        
        /// <summary>
        /// The function to be called when the event is completed. Called on the last event frame.
        /// </summary>
        public EventDelegate onEventExit = emptyDelegate;

        /// <summary>
        /// The function to be called when the event is canceled. An event can be canceled via the
        /// Escape key, or if cancelOnDoubleMousePress or cancelOnReleaseModifier is set.
        /// </summary>
        public EventDelegate onEventCancel = emptyDelegate;

        /// <summary>
        /// The function that gets called when the input requires drawing something to the screen.
        /// Used for things like drawing a selection box, or updating other visuals.
        /// </summary>
        /// <remarks>
        /// onRepaint does not have a corresponding checkedOnRepaint. This is because onRepaint
        /// should not have any code that determines an event's cancelation.
        /// </remarks>
        public EventDelegate onRepaint = emptyDelegate;

        /// <summary>
        /// The name of the UIEvent. Used for debugging purposes.
        /// </summary>
        public string name = "";

        public UIEvent(string name = "")
        {
            this.name = name;
        }
    }
}