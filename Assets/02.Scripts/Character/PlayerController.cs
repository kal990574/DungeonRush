using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Character
{
    public class PlayerController : CharacterBase
    {
        protected override void OnDamageTaken(float damage)
        {
            GameEventBus.RaisePlayerHpChanged(CurrentHp, MaxHp);
        }

        protected override void OnDeath()
        {
            GameEventBus.RaisePlayerDeath();
        }
    }
}