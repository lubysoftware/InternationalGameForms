using System;
using System.Collections.Generic;
using API;
using Proyecto26;

namespace International.Api
{
    public class GamesApi : BaseApi
    {
        public void List(int page, int amount, string filter, Action<ImageSeqList> response, Action<ErrorProxy> error)
        {
            var request = new RequestHelper
            {
                Uri = GetFormattedPath("games"),
                EnableDebug = DebugMode,
                Retries = 1,
                Timeout = 15,
                Params = new Dictionary<string, string>
                {
                    { "page", page.ToString() },
                    { "take", amount.ToString() },
                    { "search", filter }
                }
            };

            Api.Get(request, response, error);
        }

        // public void VinculateTrail(string id, Action<> response, Action<ErrorProxy> error)
        // {
        //     var request = new RequestHelper
        //     {
        //         Uri = GetFormattedPath($"games/{id}"),
        //         EnableDebug = DebugMode,
        //         Retries = 1,
        //         Timeout = 15,
        //     };
        //
        //     Api.Put(request, response, error);
        // }
    }
}