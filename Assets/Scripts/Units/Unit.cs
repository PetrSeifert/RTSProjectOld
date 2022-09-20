using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum UnitType
{
    Villager
}

public class TransformEvent : UnityEvent<Transform> {} //empty class; just needs to exist

public abstract class Unit : RTSFactionEntity, IDamageable, IAttacking
{
    public GameObject myPrefab;
    public UnitType type;
    public NavMeshAgent navMeshAgent;
    public NavMeshObstacle navMeshObstacle;
    
    [Header("Combat")]
    [SerializeField] int health;
    [SerializeField] int damage;
    [SerializeField] float reloadTime;
    [SerializeField] float attackRange;
    public int Health { get; set; }
    public int Damage { get; set; }
    public float ReloadTime { get; set; }
    public float SqrAttackRange { get; set; }
    public AttackingState AttackingState { get; set; }
    
    public bool navigationRunning;
    public UnitAction action;
    public TransformEvent onCollisionEntry = new();
    
    [HideInInspector] public float produceTime;

    protected virtual void Awake()
    {
        Health = health;
        Damage = damage;
        ReloadTime = reloadTime;
        SqrAttackRange = attackRange * attackRange;
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
    }

    protected virtual void Update()
    {
        action?.Update();
    }
    
    protected virtual void OnBecameVisible()
    {
        if (faction.localPlayerControlled)
            faction.selectionController.selectableUnits.Add(this);
    }

    protected virtual void OnBecameInvisible()
    {
        if (faction.localPlayerControlled)
            faction.selectionController.selectableUnits.Remove(this);
    }
    
    public void NavigateToTarget(Vector3 targetPosition)
    {
        if (navigationRunning)
        {
            if (navMeshAgent.pathPending) return;
            if (action.prioritySet) return;
            faction.unitController.SetUnitAgentsPriority();
            action.prioritySet = true;
        }
        else
        {
            if (!navMeshObstacle.enabled)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.destination = targetPosition;
                navigationRunning = true;
            }
            navMeshObstacle.enabled = false;
        }
    }

    public void Attack(IDamageable target)
    {
        target.TakeDamage(Damage);
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        Debug.Log(Health);
        if (Health > 0) return;
        Die();
    }

    void Die()
    {
        
    }

    protected void OnCollisionEnter(Collision collision)
    {
        onCollisionEntry.Invoke(collision.collider.transform);
    }


    public void StopNavigation()
    {
        navMeshAgent.enabled = false;
        navMeshObstacle.enabled = true;
        navigationRunning = false;
        navMeshAgent.avoidancePriority = 0;
    }

    public void SetAction(UnitAction newAction)
    {
        action?.Deactivate();
        action = newAction;
    }

    public void ToggleSelectedColor()
    {
        if (renderer.material.color == Color.grey)
        {
            renderer.material.color = Color.green;
        }
        else if (renderer.material.color == Color.green)
        {
            renderer.material.color = Color.grey;
        }
    }
}
