using System;
using API;

namespace International.Api
{
    public interface IActivityApi<out TList> where TList : BaseActivityList
    {
        public void List(int page, int amount, string filter, Action<TList> response, Action<ErrorProxy> error);
    }
}