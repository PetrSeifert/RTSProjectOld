using UnityEngine;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine.Events;

public enum UnitType
{
    Villager, Archer, Soldier, Trader
}

public class TransformEvent : UnityEvent<Transform> {} //empty class; just needs to exist

public class Unit : RTSFactionEntity, IDamageable, IAttacking
{
    public GameObject myPrefab;
    public UnitType type;
    public float produceTime;
    
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
    
    public UnitAction action;
    public TransformEvent onCollisionEntry = new();
    
    [HideInInspector] public RichAI richAI;
    

    protected override void Awake()
    {
        base.Awake();
        richAI = GetComponent<RichAI>();
        Health = health;
        Damage = damage;
        ReloadTime = reloadTime;
        SqrAttackRange = attackRange * attackRange;
        GetComponent<RVOController>().priority = Random.Range(0f, 1f);
    }

    protected override void Start()
    {
        base.Start();
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 200f, GameManager.Instance.terrainLayerMask);
        transform.position = hitInfo.point;
    }

    protected virtual void Update()
    {
        action?.Update();
        if ((richAI.velocity.magnitude < 2 || !richAI.canMove) && !richAI.pathPending &&
            !(richAI.reachedDestination || richAI.reachedEndOfPath))
        {
            richAI.canMove = true;
            richAI.SearchPath();
        }

        if (richAI.reachedDestination || richAI.reachedEndOfPath) richAI.canMove = false;
    }

    public void NavigateToTarget(Vector3 targetPosition, float endReachedDistance)
    {
        richAI.canMove = true;
        richAI.endReachedDistance = endReachedDistance;
        richAI.destination = targetPosition;
        richAI.SearchPath();
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

    protected void OnCollisionStay(Collision collision)
    {
        onCollisionEntry.Invoke(collision.collider.transform);
        RichAI otherRichAI = collision.collider.GetComponentInParent<RichAI>();
        if (otherRichAI)
            if (otherRichAI.reachedEndOfPath && otherRichAI.destination == richAI.destination)
                richAI.endReachedDistance = richAI.remainingDistance + 0.1f;
    }


    public void StopNavigation()
    {
        
    }

    public void SetAction(UnitAction newAction)
    {
        action?.Deactivate();
        action = newAction;
    }
}
