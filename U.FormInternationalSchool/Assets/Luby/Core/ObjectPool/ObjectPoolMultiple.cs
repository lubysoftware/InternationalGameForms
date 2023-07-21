using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LubyLib.Core.ObjectPool
{
    /// <summary>
    ///  <para>Object pool for instances of multiple objects.</para>
    /// </summary>
    /// <typeparam name="T">Class that inherited from BasePoolObject.</typeparam>
    [System.Serializable]
    public class ObjectPoolMultiple<T> : IObjectPoolMultiple<T> where T : BasePoolObject
    {
        private Dictionary<string, Queue<T>> _poolDictionary = new Dictionary<string, Queue<T>>();
        private Dictionary<string, List<T>> _activeDictionary = new Dictionary<string, List<T>>();

        [SerializeField] private Transform container;

        public Transform Container
        {
            get => container;
            set => container = value;
        }

        public Dictionary<string, Queue<T>> PoolDictionary => _poolDictionary;
        public Dictionary<string, List<T>> ActiveDictionary => _activeDictionary;

        public List<BasePoolObject> AllActive
        {
            get
            {
                List<BasePoolObject> list = new List<BasePoolObject>();
                foreach (var l in _activeDictionary.Values)
                {
                    list.AddRange(l);
                }

                return list;
            }
        }

        public ObjectPoolMultiple(Transform container)
        {
            this.container = container;
        }

        public virtual T GetObject(T prefab)
        {
            if (_poolDictionary == null)
            {
                _poolDictionary = new Dictionary<string, Queue<T>>();
                _activeDictionary = new Dictionary<string, List<T>>();
            }
            
            T poolObject;
            string poId = prefab.PoolObjectID;
            Queue<T> queue;
            List<T> activeList;
            if (_poolDictionary.ContainsKey(poId))
            {
                queue = _poolDictionary[poId];
                activeList = _activeDictionary[poId];
                if (queue.Count > 0)
                {
                    poolObject = queue.Dequeue();
                }
                else
                {
                    poolObject = InstantiateObject(prefab);
                }
            }
            else
            {
                AddQueueAndList(poId, out queue, out activeList);
                poolObject = InstantiateObject(prefab);
            }
            
            poolObject.Activate(() => DeactivateObject(poolObject),() => RemoveObject(poolObject));

            _activeDictionary[poId].Add(poolObject);

            return poolObject;
        }
        
        protected virtual void AddQueueAndList(string id, out Queue<T> queue, out List<T> list)
        {
            queue = new Queue<T>();
            list = new List<T>();
            _poolDictionary.Add(id, queue);
            _activeDictionary.Add(id, list);
        
        }
        
        protected virtual T InstantiateObject(T prefab)
        {
            T poolObject = Object.Instantiate(prefab, container);
            poolObject.Initialize(UniqueIDsManager.GetUniqueID(prefab.PoolObjectID + "_"));
            return poolObject;
        }

    #region Deactivation

        #region Single

        public virtual void DeactivateObject(string uniqueId)
        {
            DeactivateObject(uniqueId.Split('_')[0], uniqueId);
        }
        
        public virtual void DeactivateObject(string poolObjectId, string uniqueId)
        {
            DeactivateObject(_activeDictionary[poolObjectId].Find(x => x.UniqueID == uniqueId));
        }
        
        public virtual void DeactivateObject(T poolObject)
        {
            string poId = poolObject.PoolObjectID;
            _poolDictionary[poId].Enqueue(poolObject);
            _activeDictionary[poId].Remove(poolObject);
        }

        /*public virtual void DeactivateLastObject(T prefab)
        {
            DeactivateLastObject(prefab.PoolObjectID);
        }
        
        public virtual void DeactivateLastObject(string poId)
        {
            var list = _activeDictionary[poId];
            if(list.Count <= 0) return;
            
            var element =  list[0];
            list.RemoveAt(0);

            _activeDictionary[poId] = list;
            
            _poolDictionary[poId].Enqueue(element);
        }*/

        #endregion

        #region Multiple

        public virtual void DeactivateAllObjectsOfType(string poolObjectId)
        {
            var list = _activeDictionary[poolObjectId];
            for (int i = 0; i < list.Count; i++)
            {
                _poolDictionary[poolObjectId].Enqueue(list[i]);
            }
            _activeDictionary[poolObjectId] = new List<T>();
        }

        public virtual void DeactivateAllObjects()
        {
            var keys = _activeDictionary.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                DeactivateAllObjectsOfType(keys[i]);
            }
        }

        #endregion
        
    #endregion
        
        
    #region Removal

        #region Single

        public virtual void RemoveObject(string uniqueId)
        {
            RemoveObject(uniqueId.Split('_')[0], uniqueId);
        }
        public virtual void RemoveObject(string poolObjectId, string uniqueId)
        {
            RemoveObject(_activeDictionary[poolObjectId].Find(x => x.UniqueID == uniqueId));
        }
        
        public virtual void RemoveObject(T poolObject)
        {
            string poId = poolObject.PoolObjectID;
            _activeDictionary[poId].Remove(poolObject);
        }

        #endregion

        #region Multiple

        public virtual void RemoveAllActiveObjectsOfType(string poolObjectId)
        {
            _activeDictionary[poolObjectId] = new List<T>();
        }
        public virtual void RemoveAllInactiveObjectsOfType(string poolObjectId)
        {
            _poolDictionary[poolObjectId] = new Queue<T>();
        }
    
    
        public virtual void RemoveAllObjectsOfType(string poolObjectId)
        {
            RemoveAllActiveObjectsOfType(poolObjectId);
            RemoveAllInactiveObjectsOfType(poolObjectId);
        }
    
        public virtual void RemoveAllActiveObjects()
        {
            var keys = _activeDictionary.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                RemoveAllActiveObjectsOfType(keys[i]);
            }
        }

        public virtual void RemoveAllInactiveObjects()
        {
            var keys = _poolDictionary.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                RemoveAllInactiveObjectsOfType(keys[i]);
            }
        }

        public virtual void RemoveAllObjects()
        {
            RemoveAllActiveObjects();
            RemoveAllInactiveObjects();
        }

        #endregion
            
    #endregion
        
        
        
    }
}
