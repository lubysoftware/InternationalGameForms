using System;
using System.Collections.Generic;
using API;
using Proyecto26;

namespace International.Api
{
    public class ImageSequenceApi : BaseApi, IActivityApi<ImageSeqList>
    {
        public void List(int page, int amount, string filter, Action<ImageSeqList> response, Action<ErrorProxy> error)
        {
            var request = new RequestHelper
            {
                Uri = GetFormattedPath("image-sequence"),
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

        public void Show(string id, Action<ImageSeqJsonClass> response, Action<ErrorProxy> error)
        {
            var request = new RequestHelper
            {
                Uri = GetFormattedPath($"image-sequence/{id}"),
                EnableDebug = DebugMode,
                Retries = 1,
                Timeout = 15
            };

            Api.Get(request, response, error);
        }

        public void Create(FormImageSequence imageSequenceGame, Action<ImageSeqJsonGet> response,
            Action<ErrorProxy> error)
        {
            var request = new RequestHelper
            {
                Uri = GetFormattedPath("image-sequence"),
                Body = imageSequenceGame,
                EnableDebug = DebugMode,
                Retries = 1,
                Timeout = 15
            };

            Api.Post(request, response, error);
        }

        public void Update(string id, FormImageSequence imageSequenceGame, Action<ImageSeqJsonGet> response,
            Action<ErrorProxy> error)
        {
            var request = new RequestHelper
            {
                Uri = GetFormattedPath($"image-sequence/{id}"),
                Body = imageSequenceGame,
                EnableDebug = DebugMode,
                Retries = 1,
                Timeout = 15
            };

            Api.Put(request, response, error);
        }

        public void Delete(string id, Action<ImageSeqJsonGet> response, Action<ErrorProxy> error)
        {
            var request = new RequestHelper
            {
                Uri = GetFormattedPath($"image-sequence/{id}"),
                EnableDebug = DebugMode,
                Retries = 1,
                Timeout = 15
            };

            Api.Delete(request, response, error);
        }
    }
}