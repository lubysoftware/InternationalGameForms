using System;
using LubyLib.Core.Singletons;

public class GlobalSettings : SimpleSingleton<GlobalSettings>
{
    /// <summary>
    /// True caso o usuário abriu o formulário a partir da criação/edição de uma trilha.
    /// Usado para ativar ou desativar o botão de adicionar uma atividade à trilha que está sendo editada.
    /// </summary>
    public bool OpenedFromPath { get; private set; }

    // private void Start()
    // {
    //     OpenedFromPath = true;
    // }

    public void SetPathButtonStatus(bool status)
    {
        OpenedFromPath = status;
    }
}
