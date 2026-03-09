using UnityEngine;
using UnityEngine.Pool;

public class DamagePopupSpawner : MonoBehaviour
{
    [SerializeField] private DamagePopup _prefab;
    [SerializeField] private DamagePopupSettings _settings;

    private IObjectPool<DamagePopup> _pool;
    private static DamagePopupSpawner s_instance;

    private void Awake()
    {
        s_instance = this;
        _pool = new ObjectPool<DamagePopup>(
            createFunc: CreatePopup,
            actionOnGet: p => p.gameObject.SetActive(true),
            actionOnRelease: p => p.gameObject.SetActive(false),
            actionOnDestroy: p => Destroy(p.gameObject),
            defaultCapacity: _settings.defaultCapacity,
            maxSize: _settings.maxSize);
    }

    public static void Spawn(float damage, Vector3 position)
    {
        if (s_instance == null) return;

        DamagePopup popup = s_instance._pool.Get();
        popup.Play(damage, position);
    }

    private DamagePopup CreatePopup()
    {
        DamagePopup popup = Instantiate(_prefab, transform);
        popup.Initialize(_pool, _settings);
        return popup;
    }
}