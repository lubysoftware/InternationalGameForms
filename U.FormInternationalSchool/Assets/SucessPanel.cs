using System.Collections;
using System.Collections.Generic;
using LubyLib.Core.Singletons;
using TMPro;
using UnityEngine;

public class SucessPanel : SimpleSingleton<SucessPanel>
{
    [SerializeField] private TextMeshProUGUI textMessage;
    [SerializeField] private Transform panel;
    protected override bool DestroyOnLoad => false;

    public void SetText(string text)
    {
        textMessage.text = text;
        panel.gameObject.SetActive(true);
        Invoke(nameof(AutoDestroy),5f );
    }

    private void AutoDestroy()
    {
        panel.gameObject.SetActive(false);
    }
}
