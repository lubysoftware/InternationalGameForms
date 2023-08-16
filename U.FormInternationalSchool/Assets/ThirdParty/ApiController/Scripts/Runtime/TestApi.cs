using System;
using Proyecto26;

namespace API
{
    public class TestApi : IApi
    {
        public void Post<T, TG>(RequestHelper request, Action<T> responseCallback, Action<TG> errorCallback) where TG : ErrorProxy
        {
            responseCallback?.Invoke(default(T));
        }

        public void Get<T, TG>(RequestHelper request, Action<T> responseCallback, Action<TG> errorCallback) where TG : ErrorProxy
        {
            responseCallback?.Invoke(default(T));
        }

        public void Put<T, TG>(RequestHelper request, Action<T> responseCallback, Action<TG> errorCallback) where TG : ErrorProxy
        {
            responseCallback?.Invoke(default(T));   
        }

        public void Delete<T, TG>(RequestHelper request, Action<T> responseCallback, Action<TG> errorCallback) where TG : ErrorProxy
        {
            responseCallback?.Invoke(default(T));
        }

        public void GetArray<T, TG>(RequestHelper request, Action<T[]> responseCallback, Action<TG> errorCallback) where TG : ErrorProxy
        {
            responseCallback?.Invoke(default(T[]));
        }
    }
}