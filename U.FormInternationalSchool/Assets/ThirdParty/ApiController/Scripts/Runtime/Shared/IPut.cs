using System;
using Proyecto26;

namespace API
{
    public interface IPut
    {
        void Put<T, TG>(RequestHelper request, Action<T> responseCallback, Action<TG> errorCallback) where TG : ErrorProxy;
    }
}