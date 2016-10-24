using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Entity : MonoBehaviour {

    //Stats
    string entityName = "Mark";
    string type = "Hostile";//Rude.

    public MaxableStat health = new MaxableStat(30, 0.1f);
    public MaxableStat defence = new MaxableStat(5, 0.05f);
    public MaxableStat attack = new MaxableStat(5);
    public MaxableStat magic = new MaxableStat(5);
    public MaxableStat mana = new MaxableStat(5);
    public MaxableStat moveSpeed = new MaxableStat(5, 1.0f);
    public int tier = 0;

    void Start()
    {

    }

    protected virtual void Death()
    {
        
    }

    public virtual BehaviorState MoveTowards(Transform obj)
    {
        transform.position = Vector2.MoveTowards(transform.position, obj.position, moveSpeed.value * Time.deltaTime);
        return BehaviorState.Success;
    }

    public virtual BehaviorState MoveAwayFrom(Transform obj)
    {
        transform.position = Vector2.MoveTowards(transform.position, obj.position, -moveSpeed.value * Time.deltaTime);
        return BehaviorState.Success;
    }

    public BehaviorState PercentHealthAboveRatio(float ratio)
    {
        return (health.ratio > ratio ? BehaviorState.Success : BehaviorState.Failure);
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

public class MaxableStat
{
    public float max { get; protected set; }
    private float current;
    private float regenerationRate;

    public float value
    {
        get { return current; }
        set { current = Mathf.Min(value, max); }
    }

    public float ratio
    {
        get { return current / max; }
        set { current = Mathf.Min(value, 1.0f) * max; }
    }

    public MaxableStat(float startingValue)
    {
        max = current = startingValue;
        regenerationRate = 0;
    }

    public MaxableStat(float startingValue, float startingRegenerationRate)
    {
        max = current = startingValue;
        regenerationRate = startingRegenerationRate;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns>Returns true if maxed out.</returns>
    public bool Regenerate(float time)
    {
        current += time * regenerationRate;
        if(current >= max)
        {
            current = max;
            return true;
        }
        return false;
    }
}