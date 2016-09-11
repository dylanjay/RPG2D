using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Entity : MonoBehaviour {

    //Stats
    public string entityName = "Mark";
    public string type = "Hostile";//Rude.
    public int health = 10;
    public int maxHealth = 10;
    public int defence = 5;
    public int attack = 5;
    public int magic = 5;
    public int mana = 5;
    public float moveSpeed = 0.1f;

    void Start()
    {

    }

    protected virtual void OnDeath()
    {
        
    }

    protected virtual void MoveTowards(Transform obj)
    {
        transform.position = Vector2.MoveTowards(transform.position, obj.position, moveSpeed * Time.deltaTime);
    }

    protected virtual void MoveAwayFrom(Transform obj)
    {
        transform.position = Vector2.MoveTowards(transform.position, obj.position, -moveSpeed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {

    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {

    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {

    }

    protected virtual void OnCollisionExit2D(Collision2D other)
    {

    }

    public BehaviorState PercentHealthAboveRatio(float ratio)
    {
        return ((float)(health / maxHealth) > ratio ? BehaviorState.Success : BehaviorState.Failure);
    }

    public BehaviorState Attack()
    {
        Debug.Log("Regular Attack");
        return BehaviorState.Running;
    }
    public BehaviorState HeavyAttack()
    {
        Debug.Log("Heavy Attack");
        return BehaviorState.Running;
    }
}
