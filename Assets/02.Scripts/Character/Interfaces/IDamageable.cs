using _02.Scripts.Core;

namespace _02.Scripts.Character.Interfaces
{
    public interface IDamageable
    {
        public float CurrentHp { get; }
        public float MaxHp { get; }
        public bool IsDead { get; }

        public void TakeDamage(float damage, DamageType damageType);
        public void Heal(float amount);
    }
}