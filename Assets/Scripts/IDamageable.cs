public interface IDamageable {
    bool Damageable { get; }
    void EnableDamage();
    void DisableDamage();
}