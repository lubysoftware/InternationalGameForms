using System;
using API;
using International.Api;
using LubyLib.Core.Singletons;
using Proyecto26;
using UnityEngine;

public class ApiController : SimpleSingleton<ApiController>
{
    [SerializeField] private string authToken;
    
    private void Start()
    {
        RestClient.DefaultRequestHeaders["Authentication"] = authToken;
    }

    public void CheckHealth(Action<bool> healthStatus)
    {
        APIFactory.GetApi<UtilsApi>().CheckHealth(health =>
        {
            healthStatus?.Invoke(health.status);
        }, errorProxy =>
        {
            Debug.LogError($"Houve um erro ao checar o status do servidor: {errorProxy.statusCode} | [{errorProxy.message}]");
        });
    }
}
