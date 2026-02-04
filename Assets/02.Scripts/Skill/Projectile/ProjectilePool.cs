using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    private Projectile _prefab;
    private readonly Queue<Projectile> _pool = new Queue<Projectile>();

    public void Initialize(Projectile prefab, int initialSize = 10)
    {
        _prefab = prefab;

        for (int i = 0; i < initialSize; i++)
        {
            Projectile projectile = CreateNew();
            projectile.gameObject.SetActive(false);
            _pool.Enqueue(projectile);
        }
    }

    public Projectile Get()
    {
        Projectile projectile = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void Return(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        _pool.Enqueue(projectile);
    }

    private Projectile CreateNew()
    {
        return Instantiate(_prefab, transform);
    }
}