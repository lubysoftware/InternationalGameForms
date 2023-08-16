using System;
using Proyecto26;
using Unity.Plastic.Newtonsoft.Json;

namespace API
{
    public class RestClientAPI : IApi
    {
        public void Post<T, TG>(RequestHelper request, Action<T> responseCallback, Action<TG> errorCallback)
            where TG : ErrorProxy
        {
            RestClient.Post<T>(request)
                .Then(res => { responseCallback?.Invoke(res); })
                .Catch(err => { HandleException(errorCallback, err); });
        }

        public void Get<T, TG>(RequestHelper request, Action<T> responseCallback, Action<TG> errorCallback)
            where TG : ErrorProxy
        {
            RestClient.Get<T>(request)
                .Then(res => { responseCallback?.Invoke(res); })
                .Catch(err => { HandleException(errorCallback, err); });
        }

        public void Put<T, TG>(RequestHelper request, Action<T> responseCallback, Action<TG> errorCallback)
            where TG : ErrorProxy
        {
            RestClient.Put<T>(request)
                .Then(res => { responseCallback?.Invoke(res); })
                .Catch(err => { HandleException(errorCallback, err); });
        }

        public void Delete<T, TG>(RequestHelper request, Action<T> responseCallback, Action<TG> errorCallback)
            where TG : ErrorProxy
        {
            RestClient.Delete(request)
                .Then(res =>
                {
                    if (string.IsNullOrEmpty(res.Text))
                        responseCallback?.Invoke(default(T));

                    responseCallback?.Invoke(JsonConvert.DeserializeObject<T>(res.Text));
                })
                .Catch(err => { HandleException(errorCallback, err); });
        }

        public void GetArray<T, TG>(RequestHelper request, Action<T[]> responseCallback, Action<TG> errorCallback)
            where TG : ErrorProxy
        {
            RestClient.GetArray<T>(request)
                .Then(res => { responseCallback?.Invoke(res); })
                .Catch(err => { HandleException(errorCallback, err); });
        }

        private void HandleException<TG>(Action<TG> errorCallback, Exception err) where TG : ErrorProxy
        {
            var exception = (RequestException)err;

            if (string.IsNullOrEmpty(exception.Response))
            {
                errorCallback?.Invoke(NewErrorProxy<TG>(StatusCode.Unknown, exception));
                return;
            }

            try
            {
                var errorResponse = JsonConvert.DeserializeObject<TG>(exception.Response);
                errorCallback?.Invoke(errorResponse);
            }
            catch (Exception e)
            {
                var returnCode = StatusCode.Unknown;
                errorCallback?.Invoke(NewErrorProxy<TG>(returnCode, exception));
            }
        }

        private TG NewErrorProxy<TG>(StatusCode code, RequestException exception) where TG : ErrorProxy
        {
            return (TG)new ErrorProxy
            {
                statusCode = code,
                message = exception.Response,
                error = exception.ServerMessage,
            };
        }
    }
}