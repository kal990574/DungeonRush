namespace _02.Scripts.Character.Interfaces
{
    public interface IAttacker
    {
        public float AttackPower { get; }
        public float AttackSpeed { get; }
        public float AttackRange { get; }
        
        public void Attack(IDamageable target);
    }
}