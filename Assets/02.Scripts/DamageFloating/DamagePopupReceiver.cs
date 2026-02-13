using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class DamagePopupReceiver : MonoBehaviour
{
    private HealthComponent _health;

    private void Awake()
    {
        _health = GetComponent<HealthComponent>();
    }

    private void OnEnable()
    {
        _health.OnDamaged += HandleDamaged;
    }

    private void OnDisable()
    {
        _health.OnDamaged -= HandleDamaged;
    }

    private void HandleDamaged(float damage)
    {
        DamagePopupSpawner.Spawn(damage, transform.position);
    }
}