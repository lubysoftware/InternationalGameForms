using System.Collections.Generic;
using UnityEngine;

namespace LubyLib.Core.ObjectPool
{
    /// <summary>
    ///     <para>Object pool for instances of a single object.</para>
    /// </summary>
    /// <typeparam name="T">Class that inherited from BasePoolObject.</typeparam>
    [System.Serializable]
    public class ObjectPoolSingle<T> : IObjectPoolSingle<T> where T : BasePoolObject
    {
        public T prefab;
        [SerializeField] private Transform container;
        [ReadOnly, SerializeField] private List<T> _active = new List<T>();
        private Queue<T> _pool = new Queue<T>();
        
        public Transform Container
        {
            get => container;
            set => container = value;
        }

        public List<T> Active => _active;

        public ObjectPoolSingle(T prefab, Transform container)
        {
            this.prefab = prefab;
            this.container = container;
        }

        public virtual T GetObject()
        {
            T poolObject;

            if (_pool.Count > 0)
                poolObject = _pool.Dequeue();
            else
                poolObject = Object.Instantiate(prefab, container);

            _active.Add(poolObject);
            
            poolObject.Activate(() => DeactivateObject(poolObject),() => RemoveObject(poolObject));

            return poolObject;
        }

        #region Deactivation

        public virtual void DeactivateObject(string uniqueId)
        {
            DeactivateObject(_active.Find(x => x.UniqueID == uniqueId));
        }
        public virtual void DeactivateObject(T element)
        {
            _active.Remove(element);
            _pool.Enqueue(element);
        }

        public virtual void DeactivateLastObject()
        {
            if(_active.Count <= 0) return;
            var element = _active[0];
            _active.RemoveAt(0);
            _pool.Enqueue(element);
        }

        public virtual void DeactivateAllObjects()
        {
            for (int i = 0; i < _active.Count; i++)
            {
                _active[i].gameObject.SetActive(false);
                _pool.Enqueue(_active[i]);
            }

            _active = new List<T>();
        }

        #endregion
        
        #region Removal

        public virtual void RemoveObject(string uniqueId)
        {
            RemoveObject(_active.Find(x => x.UniqueID == uniqueId));
        }
        
        public virtual void RemoveObject(T poolObject)
        {
            if (poolObject.IsActive)
            {
                _active.Remove(poolObject);
            }
            else
            {
                List<T> list = new List<T>();
                for (int i = 0; i < _pool.Count; i++)
                {
                    list.Add(_pool.Dequeue());
                }

                list.Remove(poolObject);
                for (int i = 0; i < list.Count; i++)
                {
                    _pool.Enqueue(list[i]);
                }
            }
        }

        public void RemoveActiveObjects()
        {
            _active = new List<T>();
        }

        public virtual void RemoveInactiveObjects()
        {
            _pool = new Queue<T>();
        }
        
        public virtual void RemoveAllObjects()
        {
            RemoveActiveObjects();
            RemoveInactiveObjects();
        }

        #endregion
        
    }

}