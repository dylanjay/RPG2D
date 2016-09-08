using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using RPG2DAttributes;
using UnityEngine.UI;

public class BehaviorSlot : MonoBehaviour, IDragHandler {

    /*public class BehaviorFunc<T> : UnityFunc<T, BehaviorState> { }
    [Serializable]
    public class BehaviorFuncFloat : BehaviorFunc<float> { }
    [Serializable]
    public class BehaviorFuncString : BehaviorFunc<string> { }
    [Serializable]
    public class BehaviorFuncInt : BehaviorFunc<int> { }
    [Serializable]
    public class BehaviorFuncGameObject : BehaviorFunc<GameObject> { }*/

    public GameObject parentObject;
    

    public BehaviorComponent behaviorComponent;

    public Type parameterType;

    /*[HideInInspector]
    [SerializeField]
    public object behaviorFunc = new BehaviorFuncGameObject();*/

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

    /*[ShowDelegate("Action")]
    public object action = new BehaviorFuncGameObject();*/

    public void OnDrag(PointerEventData eventData)
    {
        transform.Translate(eventData.delta);
    }

}
