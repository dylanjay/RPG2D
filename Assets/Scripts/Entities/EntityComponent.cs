using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EntityComponent : MonoBehaviour {

    public Entity entity;

    public int entityID = 0;

    void Start()
    {
        entity = EntityDatabase.instance.GetEntityByID(entityID);
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

    public BehaviorState PercentHealthAboveRatio(float ratio)
    {
        return (ratio < .5f ? BehaviorState.Success : BehaviorState.Failure);
    }

    public BehaviorState Attack()
    {
        return BehaviorState.Running;
    }
    public BehaviorState HeavyAttack()
    {
        return BehaviorState.Running;
    }
}
