using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    private EffectAutoReturn _prefab;
    private readonly Queue<EffectAutoReturn> _pool = new Queue<EffectAutoReturn>();

    public void Initialize(EffectAutoReturn prefab, int initialSize = 5)
    {
        _prefab = prefab;

        for (int i = 0; i < initialSize; i++)
        {
            EffectAutoReturn effect = CreateNew();
            effect.gameObject.SetActive(false);
            _pool.Enqueue(effect);
        }
    }

    public void Play(Vector3 position)
    {
        EffectAutoReturn effect = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
        effect.transform.position = position;
        effect.gameObject.SetActive(true);
    }

    private void Return(EffectAutoReturn effect)
    {
        effect.gameObject.SetActive(false);
        _pool.Enqueue(effect);
    }

    private EffectAutoReturn CreateNew()
    {
        EffectAutoReturn effect = Instantiate(_prefab, transform);
        effect.OnFinished += Return;
        return effect;
    }
}