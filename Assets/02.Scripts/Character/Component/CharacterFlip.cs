using UnityEngine;

public class CharacterFlip : MonoBehaviour
{
    [SerializeField] private Transform _flipTarget;

    [SerializeField] private bool _facingRight = true;

    public void FaceDirection(Vector2 direction)
    {
        if (direction.x > 0 && !_facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && _facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 scale = _flipTarget.localScale;
        scale.x *= -1;
        _flipTarget.localScale = scale;
    }
}