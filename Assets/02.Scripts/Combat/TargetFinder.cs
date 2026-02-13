using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private float _detectRange = 10f;
    [SerializeField] private LayerMask _targetLayer;

    private readonly Collider2D[] _results = new Collider2D[20];
    private ContactFilter2D _contactFilter;

    public void Initialize(float detectRange)
    {
        _detectRange = detectRange;
    }

    private void Awake()
    {
        _contactFilter = new ContactFilter2D();
        _contactFilter.SetLayerMask(_targetLayer);
        _contactFilter.useLayerMask = true;
    }

    public Transform FindNearestTarget()
    {
        return FindNearestFrom(transform.position);
    }

    public Transform FindNearestFrom(Vector2 position)
    {
        int count = Physics2D.OverlapCircle(
            position, _detectRange, _contactFilter, _results);

        if (count == 0) return null;

        Transform nearest = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var damageable = _results[i].GetComponent<IDamageable>();
            if (damageable == null || !damageable.IsAlive) continue;

            float distance = Vector2.Distance(position, _results[i].transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = _results[i].transform;
            }
        }

        return nearest;
    }
}