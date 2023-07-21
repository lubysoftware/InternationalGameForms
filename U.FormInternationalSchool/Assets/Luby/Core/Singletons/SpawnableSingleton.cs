using UnityEngine;

namespace LubyLib.Core.Singletons
{
    /// <summary>
    /// <para>Singleton that spawns an Instance whenever there is not one. That Instance is public.</para>
    /// <remarks>
    /// <c>ShouldOverride: </c>Decides if a new Instance overrides a pre-existing one. Default is false.
    /// <c>DestroyOnLoad: </c>Decides if the object should or not be destroyed on load. Default is true.
    /// </remarks>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpawnableSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected virtual bool ShouldOverride => false;
        protected virtual bool DestroyOnLoad => true;
        
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }

                return _instance;
            }
        }
        
        protected virtual void Awake()
        {
            if(_instance != null)
            {
                if (ShouldOverride)
                {
                    Destroy(_instance.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
            }

            _instance = this as T;
            
            if(!DestroyOnLoad)
                DontDestroyOnLoad(gameObject);
            
        }
    }
}
