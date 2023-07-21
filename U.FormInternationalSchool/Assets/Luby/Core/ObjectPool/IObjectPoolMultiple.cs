using UnityEngine;

namespace LubyLib.Core.ObjectPool
{
    public interface IObjectPoolMultiple<T>
    {
        public Transform Container { get; set; }
        public T GetObject(T prefab);
        public void DeactivateObject(string uniqueId);
        public void DeactivateObject(string poolObjectId, string uniqueId);
        public void DeactivateObject(T poolObject);
        public void DeactivateAllObjectsOfType(string poolObjectId);
        public void DeactivateAllObjects();
        public void RemoveObject(string uniqueId);
        public void RemoveObject(string poolObjectId, string uniqueId);
        public void RemoveObject(T poolObject);
        public void RemoveAllActiveObjectsOfType(string poolObjectId);
        public void RemoveAllInactiveObjectsOfType(string poolObjectId);
        public void RemoveAllObjectsOfType(string poolObjectId);
        public void RemoveAllActiveObjects();
        public void RemoveAllInactiveObjects();
        public void RemoveAllObjects();

    }
}
