using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using IPoolable = _02.Scripts.Character.Interfaces.IPoolable;

namespace _02.Scripts.Core
{
    public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
    {
        [Header("Pool Settings")] 
        [SerializeField] private T _prefab;
        [SerializeField] private int _initialSize = 10;
        [SerializeField] private Transform _poolParent;

        private Queue<T> _pool = new Queue<T>();
        public int PooledCount => _pool.Count;
        private void Awake()
        {
            Prewarm(_initialSize);
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T obj = CreateNew();
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            T obj = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
            obj.gameObject.SetActive(true);
            obj.OnSpawn();
            return obj;
        }

        public void Return(T obj)
        {
            obj.OnDespawn();
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }

        private T CreateNew()
        {
            Transform parent = _poolParent != null ? _poolParent : transform;
            T obj = Instantiate(_prefab, parent);
            return obj;
        }
    }
}