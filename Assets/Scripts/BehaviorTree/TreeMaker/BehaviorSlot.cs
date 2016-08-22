using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using RPG2DAttributes;
using UnityEngine.UI;

public class BehaviorSlot : MonoBehaviour, IDragHandler {

    public GameObject parentObject;

    public BehaviorComponent behaviorComponent;

    private string _nodeName = "";
    [ShowStringProperty]
    public string nodeName
    {
        get
        {
            return _nodeName;
        }
        set
        {
            if (behaviorComponent == null) { return; }
            _nodeName = value;
            behaviorComponent.name = _nodeName;
            GetComponentInChildren<Text>().text = _nodeName;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.Translate(eventData.delta);
    }

}
