using UnityEngine;

namespace LubyLib.Core.ObjectPool
{
    public interface IObjectPoolSingle<T>
    {
        public Transform Container { get; set; }
        public T GetObject();
        public void DeactivateObject(string uniqueId);
        public void DeactivateObject(T element);
        public void DeactivateAllObjects();
        public void RemoveObject(string uniqueId);
        public void RemoveObject(T poolObject);
        public void RemoveActiveObjects();
        public void RemoveInactiveObjects();
        public void RemoveAllObjects();
    }
}
