using System;
using Proyecto26;

namespace API
{
    public interface IDelete
    {
        void Delete<T, TG>(RequestHelper request, Action<T> responseCallback, Action<TG> errorCallback) where TG : ErrorProxy;
    }
}