using System.Collections.Generic;
using System.Runtime.InteropServices;
using LubyLib.Core.Singletons;
using UnityEngine;

public class PortalBridge : SimpleSingleton<PortalBridge>
{
    [DllImport("__Internal")]
    protected static extern void OnGameCreated(int gameId, string gameType);
    
    [DllImport("__Internal")]
    protected static extern void OnShowPreview(string gameType, string json);

    [DllImport("__Internal")]
    protected static extern void AddGameToPath(int gameId, string gameType);

    public void OnGameCreatedEvent(string responseJson)
    {
        Debug.Log("[DEBUG] Informação do game enviada para o portal.");
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            GameCreationResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<GameCreationResponse>(responseJson);
            OnGameCreated(response.id, response.gameType);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
#endif
    }

    public void OnShowPreviewEvent(string gameType, string jsonData)
    {
        Debug.Log("[DEBUG] Solicitacao preview para o portal.");
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            OnShowPreview(gameType, jsonData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
#endif
    }

    public void AddGameToPathEvent(int gameId, string gameType)
    {
        Debug.Log($"[DEBUG] Id do [{gameId}] jogo enviado para ser adicionado na trilha");
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            AddGameToPath(gameId, gameType);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
#endif
    }
}
