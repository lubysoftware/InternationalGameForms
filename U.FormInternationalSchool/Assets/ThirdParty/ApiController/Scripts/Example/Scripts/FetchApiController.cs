using System;
using Proyecto26;

namespace API.Example
{
    /// <summary>
    /// From https://jsonplaceholder.typicode.com/guide/
    /// </summary>
    public class FetchApiController : BaseApi
    {
        public void Get(Action<ExampleModel> response, Action<ErrorProxy> onError)
        {
            var currentRequest = new RequestHelper
            {
                Uri = GetFormattedPath("1"),
                EnableDebug = DebugMode,
                Retries = 3,
                Timeout = 15
            };
            
            Api.Get(currentRequest, response, onError);
        }
        
        public void Create(ExampleModel model, Action<ExampleModel> response, Action<ErrorProxy> onError)
        {
            var currentRequest = new RequestHelper
            {
                Uri = _basePath,
                Body = model,
                EnableDebug = DebugMode,
                Retries = 3,
                Timeout = 15
            };
            
            Api.Post(currentRequest, response, onError);
        }
        
        public void Update(ExampleModel model, Action<ExampleModel> response, Action<ErrorProxy> onError)
        {
            var currentRequest = new RequestHelper
            {
                Uri = GetFormattedPath("1"),
                Body = model,
                EnableDebug = DebugMode,
                Retries = 3,
                Timeout = 15
            };
            
            Api.Put(currentRequest, response, onError);
        }
    }
}