using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private float _detectRange = 10f;
    [SerializeField] private LayerMask _targetLayer;

    private readonly Collider2D[] _results = new Collider2D[20];

    public void Initialize(float detectRange)
    {
        _detectRange = detectRange;
    }

    public Transform FindNearestTarget()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position, _detectRange, _results, _targetLayer);

        if (count == 0) return null;

        Transform nearest = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            float distance = Vector2.Distance(
                transform.position, _results[i].transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = _results[i].transform;
            }
        }

        return nearest;
    }
}