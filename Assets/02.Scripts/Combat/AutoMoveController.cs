using UnityEngine;

public class AutoMoveController : MonoBehaviour
{
    private float _moveSpeed;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

    public void MoveTo(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        _rigidbody.linearVelocity = direction * _moveSpeed;
    }

    public void Stop()
    {
        _rigidbody.linearVelocity = Vector2.zero;
    }
}