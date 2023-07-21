using UnityEngine;

namespace LubyLib.Core.Singletons
{
    /// <summary>
    /// <para>Basic singleton that has a public Instance.</para>
    /// <remarks>
    /// <c>ShouldOverride: </c>Decides if a new Instance overrides a pre-existing one. Default is false.
    /// <c>DestroyOnLoad: </c>Decides if the object should or not be destroyed on load. Default is true.
    /// </remarks>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected virtual bool ShouldOverride => false;
        protected virtual bool DestroyOnLoad => true;
        
        public static T Instance { get; private set; }

        
        protected virtual void Awake()
        {
            if (Instance != null)
            {
                if (ShouldOverride)
                {
                    Destroy(Instance.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
            }
            
            Instance = this as T;
            
            if(!DestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
    }
}