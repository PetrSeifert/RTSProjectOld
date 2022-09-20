public interface IAttacking
{
    AttackingState AttackingState { get; set; }
    float SqrAttackRange { get; set; }
    float ReloadTime { get; set; }
    int Damage { get; set; }
    void Attack(IDamageable target);
}
