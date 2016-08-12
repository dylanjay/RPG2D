using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EntityComponent : MonoBehaviour {

    public Entity entity;

    public int entityID = 0;

    void Start()
    {
        entity = EntityDatabase.instance.FetchEntityByID(entityID);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        entity.OnTriggerEnter2D(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        entity.OnTriggerExit2D(other);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        entity.OnCollisionEnter2D(other);
    }

   void OnCollisionExit2D(Collision2D other)
    {
        entity.OnCollisionEnter2D(other);
    }
}
