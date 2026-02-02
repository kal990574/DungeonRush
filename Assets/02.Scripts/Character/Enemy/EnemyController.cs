using _02.Scripts.Character.Interfaces;
using _02.Scripts.Core;
using _02.Scripts.Data.Enemy;
using UnityEngine;

namespace _02.Scripts.Character.Enemy
{
    public class EnemyController : CharacterBase, IPoolable
    {
        private EnemyData _data;
        private Transform _target;
        private float _attackTimer;

        public EnemyData Data => _data;

        public void Setup(EnemyData data, Transform target)
        {
            _data = data;
            _target = target;
            _maxHp = data.maxHp;
            _currentHp = _maxHp;
        }

        private void Update()
        {
            if (IsDead || _target == null) return;

            float distance = Vector2.Distance(transform.position, _target.position);

            if (distance <= _data.attackRange)
            {
                Attack();
            }
            else
            {
                MoveToTarget();
            }
        }

        private void MoveToTarget()
        {
            SetState(CharacterState.Running);
            var direction = (_target.position - transform.position).normalized;
            transform.position += direction * (_data.moveSpeed * Time.deltaTime);
        }

        private void Attack()
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer > 0f) return;

            _attackTimer = 1f / _data.attackSpeed;
            SetState(CharacterState.Attacking);

            var targetDamageable = _target.GetComponent<IDamageable>();
            targetDamageable?.TakeDamage(_data.attackDamage, DamageType.Physical);
        }

        protected override void OnDamageTaken(float damage)
        {
            // 프로토: 피격 이펙트 추후 추가.
        }

        protected override void OnDeath()
        {
            GameEventBus.RaiseEnemyKilled(this, _data.xpReward);
        }

        public void OnSpawn()
        {
            _attackTimer = 0f;
            SetState(CharacterState.Running);
        }

        public void OnDespawn()
        {
            _data = null;
            _target = null;
        }
    }
}
