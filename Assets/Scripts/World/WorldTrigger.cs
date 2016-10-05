using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class WorldTrigger : MonoBehaviour {

    public bool disableThisOnTouch = false;

    public UnityEvent actionsOnEnter;
    
    public UnityEvent actionsOnExit;

    [TextArea(3, 10)]
    public string comments;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != Player.instance.gameObject) { return; }
        actionsOnEnter.Invoke();
        if (disableThisOnTouch) { gameObject.SetActive(false); }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject != Player.instance.gameObject) { return; }
        actionsOnExit.Invoke();
    }
}
