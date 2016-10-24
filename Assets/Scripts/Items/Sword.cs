using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {
    
    //TEMPORARY REMOVE LATER
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Hostile>())
        {
            other.GetComponent<Hostile>().TakeDamage(5);
        }
    }
}
