using System;
using UnityEngine;

namespace LubyLib.Core.ObjectPool
{
    public class BasePoolObject : MonoBehaviour, IPoolObject
    {
        protected bool _active;
        [SerializeField] private string prefabId = "";
        [ReadOnly, SerializeField] private string uniqueID = "";
        private Action deactivateAction;
        private Action destroyAction;

        
        public bool IsActive => _active;
        public string PoolObjectID => prefabId;
        public string UniqueID => uniqueID;
        
        
        public void Initialize(string uniqueId)
        {
            uniqueID = uniqueId;
        }

        public void Activate(Action deactivateAction = null, Action destroyAction = null)
        {
            this.deactivateAction = deactivateAction;
            this.destroyAction = destroyAction;
            _active = true;
            gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
	        if(!_active) return;
            _active = false;
            gameObject.SetActive(false);
            deactivateAction?.Invoke();
        }

        public virtual void Delete()
        {
            _active = false;
            destroyAction?.Invoke();
            Destroy(gameObject);
        }
    }
}
