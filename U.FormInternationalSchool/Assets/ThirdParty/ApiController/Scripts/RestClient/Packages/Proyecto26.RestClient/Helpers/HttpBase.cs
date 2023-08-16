using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Proyecto26.Common;

namespace Proyecto26
{
    public static class HttpBase
    {
        public static IEnumerator CreateRequestAndRetry(RequestHelper options,
            Action<RequestException, ResponseHelper> callback)
        {
            var retries = 0;
            do
            {
                using var request = CreateRequest(options);
                yield return request.SendWebRequestWithOptions(options);
                var response = request.CreateWebResponse();
                if (request.IsValidRequest(options))
                {
                    DebugLog(options.EnableDebug,
                        $"RestClient - Response\nUrl: {options.Uri}\nMethod: {options.Method}\nStatus: {request.responseCode}\nResponse: {(options.ParseResponseBody ? response.Text : "body not parsed")}",
                        false);
                    callback(null, response);
                    break;
                }
                
                if (!options.IsAborted && retries < options.Retries && request.isNetworkError)
                {
                    yield return new WaitForSeconds(options.RetrySecondsDelay);
                    retries++;
                    if (options.RetryCallback != null)
                    {
                        options.RetryCallback(CreateException(options, request), retries);
                    }

                    DebugLog(options.EnableDebug,
                        $"RestClient - Retry Request\nUrl: {options.Uri}\nMethod: {options.Method}", false);
                }
                else
                {
                    var err = CreateException(options, request);
                    DebugLog(options.EnableDebug, err, true);
                    callback(err, response);
                    break;
                }
            } while (retries <= options.Retries);
        }

        private static UnityWebRequest CreateRequest(RequestHelper options)
        {
            var url = options.Uri.BuildUrl(options.Params);
            DebugLog(options.EnableDebug, $"RestClient - Request\nUrl: {url}", false);
            if (options.FormData is WWWForm && options.Method == UnityWebRequest.kHttpVerbPOST)
            {
                return UnityWebRequest.Post(url, options.FormData);
            }

            return new UnityWebRequest(url, options.Method);
        }

        private static RequestException CreateException(RequestHelper options, UnityWebRequest request)
        {
            return new RequestException(request.error, request.isHttpError, request.isNetworkError,
                request.responseCode, options.ParseResponseBody ? request.downloadHandler.text : "body not parsed");
        }

        private static void DebugLog(bool debugEnabled, object message, bool isError)
        {
            if (!debugEnabled) return;

            if (isError)
            {
                Debug.LogError(message);
            }
            else
            {
                Debug.Log(message);
            }
        }

        public static IEnumerator DefaultUnityWebRequest(RequestHelper options,
            Action<RequestException, ResponseHelper> callback)
        {
            return CreateRequestAndRetry(options, callback);
        }

        public static IEnumerator DefaultUnityWebRequest<TResponse>(RequestHelper options,
            Action<RequestException, ResponseHelper, TResponse> callback)
        {
            return CreateRequestAndRetry(options, (RequestException err, ResponseHelper res) =>
            {
                var body = default(TResponse);
                try
                {
                    if (err == null && res.Data != null && options.ParseResponseBody)
                    {
                        body = JsonConvert.DeserializeObject<TResponse>(res.Text);
                    }
                }
                catch (Exception error)
                {
                    DebugLog(options.EnableDebug, $"RestClient - Invalid JSON format\nError: {error.Message}", true);
                    err = new RequestException(error.Message);
                }
                finally
                {
                    callback(err, res, body);
                }
            });
        }

        public static IEnumerator DefaultUnityWebRequest<TResponse>(RequestHelper options,
            Action<RequestException, ResponseHelper, TResponse[]> callback)
        {
            return CreateRequestAndRetry(options, (RequestException err, ResponseHelper res) =>
            {
                var body = default(TResponse[]);
                try
                {
                    if (err == null && res.Data != null && options.ParseResponseBody)
                    {
                        body = JsonHelper.ArrayFromJson<TResponse>(res.Text);
                    }
                }
                catch (Exception error)
                {
                    DebugLog(options.EnableDebug, $"RestClient - Invalid JSON format\nError: {error.Message}", true);
                    err = new RequestException(error.Message);
                }
                finally
                {
                    callback(err, res, body);
                }
            });
        }
    }
}