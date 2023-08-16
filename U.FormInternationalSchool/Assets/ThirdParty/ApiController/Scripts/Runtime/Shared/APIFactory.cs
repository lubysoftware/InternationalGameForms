using System;
using System.Collections.Generic;
using System.Linq;

namespace API
{
    public static class APIFactory
    {
        private static Dictionary<Type, BaseApi> _apis;
        private static bool _isInitialized => _apis != null;

        public static void Initialize(string basePath, IApi post, bool debugMode = false)
        {
            if (_isInitialized) return;

            var apisTypes = new List<Type>();
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                apisTypes.AddRange(assembly.GetTypes().Where(myApi =>
                    myApi.IsClass && !myApi.IsAbstract && myApi.IsSubclassOf(typeof(BaseApi))));
            }

            _apis = new Dictionary<Type, BaseApi>();

            foreach (var type in apisTypes)
            {
                var tempApi = Activator.CreateInstance(type) as BaseApi;
                
                if (tempApi == null) continue;
                
                tempApi.Initialize(basePath, post, debugMode);
                
                _apis.Add(type, tempApi);
            } 
        }

        public static T GetApi<T>() where T : BaseApi
        {
            if (!_isInitialized)
            {
                Initialize("", new TestApi(), true);
            }

            _apis.TryGetValue(typeof(T), out var item);

            return (T) item;
        }
    }
}