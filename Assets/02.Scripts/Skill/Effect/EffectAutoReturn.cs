using System;
using UnityEngine;

public class EffectAutoReturn : MonoBehaviour
{
    private Animator _animator;

    public event Action<EffectAutoReturn> OnFinished;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _animator.Play(0, 0, 0f);
    }

    private void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            OnFinished?.Invoke(this);
        }
    }
}