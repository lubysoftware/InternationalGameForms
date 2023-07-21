using System;

namespace LubyLib.Core.ObjectPool
{
    public interface IPoolObject
    {
        public bool IsActive { get; }
        public string PoolObjectID{ get; }
        public string UniqueID{ get; }
        public void Initialize(string uniqueId);
        public void Activate(Action deactivateAction = null, Action destroyAction = null);
        public void Deactivate();
        public void Delete();
    }
    
}
