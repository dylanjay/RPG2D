using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    //TEMPORARY REMOVE LATER
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Hostile>())
        {
            other.GetComponent<Hostile>().takeDamage(5);
        }
    }
}
