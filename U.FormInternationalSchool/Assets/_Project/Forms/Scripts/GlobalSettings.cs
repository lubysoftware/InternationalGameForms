using System;
using LubyLib.Core.Singletons;
using UnityEngine;

public class GlobalSettings : SimpleSingleton<GlobalSettings>
{
    /// <summary>
    /// True caso o usuário abriu o formulário a partir da criação/edição de uma trilha.
    /// Usado para ativar ou desativar o botão de adicionar uma atividade à trilha que está sendo editada.
    /// </summary>
    public bool OpenedFromPath { get; private set; }

    public string UserToken
    {
        get
        {
#if UNITY_EDITOR
            return "Bearer Luby2021";
#else
            return _userToken;
#endif
        }
        private set => _userToken = value;
    }

    private string _userToken;
    
    protected override bool DestroyOnLoad => false;

    public void Setup(string setupConfig)
    {
        SetupConfig setup = JsonUtility.FromJson<SetupConfig>(setupConfig);
        
        UserToken = setup.userToken;
        OpenedFromPath = setup.openedFromPath;
    }
}

[Serializable]
public class SetupConfig
{
    public string userToken;
    public bool openedFromPath;
}