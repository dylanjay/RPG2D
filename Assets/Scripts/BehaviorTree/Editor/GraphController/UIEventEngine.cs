﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benco.Graph
{
    public class UIEventEngine : MonoBehaviour
    {
        /// <summary>
        /// A small class that contains the current OnGUI Event state.
        /// </summary>
        private class EventState
        {
            public ModifierKeys modifiers { get; set; }
            public MouseButtons mouseButtons { get; set; }
            public EventType eventType { get; set; }
            public string eventCommand { get; set; }
            /// <summary>
            /// Updates the EventState from a given Event.
            /// </summary>
            /// <param name="e">The OnGUI Event that occured.</param>
            public void UpdateEvent(Event e)
            {
                // Sum up the modification buttons
                modifiers = 0 | (e.control ? ModifierKeys.Control : 0)
                              | (e.alt ? ModifierKeys.Alt : 0)
                              | (e.shift ? ModifierKeys.Shift : 0);
                // Add ModifierKeys.None if no modifier key was pressed.
                if (modifiers == 0)
                {
                    modifiers = ModifierKeys.None;
                }

                eventCommand = e.commandName;
                eventType = e.type;

                // If a mouse button was pressed, update currentEventState.mouseButtons.
                if (e.type == EventType.MouseDown)
                {
                    mouseButtons |= (MouseButtons)(1 << e.button);
                }
            }
        }

        private EventState currentEventState = new EventState();
        UIEvent currentEvent = null;

        public Event lastMouseEvent { get; private set; }
        public Event lastKeyEvent { get; private set; }

        /// <summary>
        /// The list of possible events to parse.
        /// </summary>
        public List<UIEvent> eventList { get; set; }

        public UIEventEngine() { }

        /// <summary>
        /// If the event is not generated by the end-user, handle it and return true.
        /// </summary>
        private bool HandleNonUserEvent(Event e)
        {
            if (e.type == EventType.Repaint)
            {
                if (currentEvent != null)
                {
                    currentEvent.onRepaint(e);
                }
                return true;
            }
            return e.type == EventType.Layout || e.type == EventType.Used;
        }

        /// <summary>
        /// Cancels an event and stops tracking its state changes.
        /// </summary>
        /// <param name="e">The event to send to the canceled UIEvent.</param>
        private void CancelEvent(Event e)
        {
            currentEvent.onEventCancel(e);
            currentEvent = null;
            lastMouseEvent = null;
            lastKeyEvent = null;
        }

        private bool HasCorrectModifiers(EventState eventState, UIEvent uiEvent)
        {
            if (uiEvent.mustHaveAllModifiers)
            {
                return (uiEvent.modifiers == eventState.modifiers);
            }
            else
            {
                return (uiEvent.modifiers & eventState.modifiers) > 0;
            }
        }

        private bool HasCorrectMouseButtons(EventState eventState, UIEvent uiEvent)
        {
            if (uiEvent.mustHaveAllMouseButtons)
            {
                return uiEvent.mouseButtons == eventState.mouseButtons;
            }
            else
            {
                return (uiEvent.mouseButtons & eventState.mouseButtons) > 0;
            }
        }

        private bool HasCorrectEvent(EventState eventState, UIEvent uiEvent)
        {
            if (uiEvent.eventType == EventType.ValidateCommand ||
                uiEvent.eventType == EventType.ExecuteCommand)
            {
                if (eventState.eventType == EventType.ValidateCommand ||
                eventState.eventType == EventType.ExecuteCommand)
                {
                    string[] commands = uiEvent.eventCommand.Split('|');
                    for (int i = 0; i < commands.Length; i++)
                    {
                        if (commands[i] == eventState.eventCommand)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else if (uiEvent.eventType == EventType.MouseDrag)
            {
                return lastMouseEvent != null &&
                       lastMouseEvent.type == EventType.MouseDown &&
                       eventState.eventType == EventType.MouseDrag;
            }
            else
            {
                return eventState.eventType == uiEvent.eventType;
            }
        }

        /// <summary>
        /// Resets and clears the internal state.
        /// </summary>
        public void Reset()
        {
            lastMouseEvent = null;
            lastKeyEvent = null;
            currentEventState = new EventState();
            currentEvent = null;
        }

        /// <summary>
        /// Parses OnGUI Events to find out which UIEvent to run.
        /// </summary>
        /// <param name="e"></param>
        public void OnGUI(Event e)
        {
            if (HandleNonUserEvent(e))
            {
                return;
            }
            currentEventState.UpdateEvent(e);

            bool newEvent = false;
            if (currentEvent == null)
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    UIEvent potentialEvent = eventList[i];
                    if (HasCorrectModifiers(currentEventState, potentialEvent) &&
                        HasCorrectMouseButtons(currentEventState, potentialEvent) &&
                        HasCorrectEvent(currentEventState, potentialEvent))
                    {
                        currentEvent = potentialEvent;
                        newEvent = true;
                        break;
                    }
                }
            }
            // Note: not same if statement because currentEvent can be set in the previous if statement.
            if (currentEvent != null)
            {
                if (newEvent)
                {
                    if (currentEvent.eventType == EventType.MouseDrag)
                    {
                        if (!currentEvent.checkedOnEventBegin(lastMouseEvent) ||
                            !currentEvent.checkedOnEventUpdate(e))
                        {
                            CancelEvent(e);
                        }
                    }
                    else if (currentEvent.eventType == EventType.MouseDown ||
                             currentEvent.eventType == EventType.MouseUp ||
                             currentEvent.eventType == EventType.ValidateCommand ||
                             currentEvent.eventType == EventType.ExecuteCommand ||
                             currentEvent.eventType == EventType.ScrollWheel)
                    {
                        if (!currentEvent.checkedOnEventBegin(e) ||
                            !currentEvent.checkedOnEventUpdate(e))
                        {
                            CancelEvent(e);
                        }
                        else
                        {
                            currentEvent.onEventExit(e);
                        }
                        currentEvent = null;
                    }
                    else
                    {
                        if (!currentEvent.checkedOnEventBegin(e) ||
                            !currentEvent.checkedOnEventUpdate(e))
                        {
                            CancelEvent(e);
                        }
                    }
                }
                else
                {
                    if ((e.type == EventType.MouseUp || e.type == EventType.Ignore) &&
                        currentEvent.eventType == EventType.MouseDrag)
                    {
                        // Return value is ignored here because the event is exiting anyway.
                        currentEvent.checkedOnEventUpdate(e);
                        currentEvent.onEventExit(e);
                        currentEvent = null;
                        lastMouseEvent = null;
                    }
                    else if ((currentEventState.mouseButtons == MouseButtons.Both &&
                              currentEvent.cancelOnBothMouseButtonsPressed) || e.keyCode == KeyCode.Escape)
                    {
                        CancelEvent(e);
                    }
                    else
                    {
                        if (!currentEvent.checkedOnEventUpdate(e))
                        {
                            CancelEvent(e);
                        }
                    }
                }
            }
            // If a button was released, remove it for the next event frame.
            if (e.type == EventType.MouseUp)
            {
                currentEventState.mouseButtons &= (MouseButtons)~(1 << e.button);
            }
            if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown || e.type == EventType.MouseUp)
            {
                lastMouseEvent = new Event(e);
            }
            else if (e.type == EventType.KeyDown || e.type == EventType.KeyUp)
            {
                lastKeyEvent = new Event(e);
            }
        }
    }
}