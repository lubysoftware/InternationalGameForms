using LubyLib.Core.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuccessPanel : SimpleSingleton<SuccessPanel>
{
    [SerializeField] private TextMeshProUGUI textMessage;
    [SerializeField] private Transform panel;
    [SerializeField] private Sprite[] backgrounds;
    [SerializeField] private Image image;
    protected override bool DestroyOnLoad => false;

    public enum MessageType
    {
        ERROR,
        SUCCESS
    }

    public void SetText(string text, MessageType type)
    {
        textMessage.text = text;
        GetMessageType(type);
        panel.gameObject.SetActive(true);
        Invoke(nameof(AutoDestroy), 5f);
    }

    private void AutoDestroy()
    {
        panel.gameObject.SetActive(false);
    }

    private void GetMessageType(MessageType type)
    {
        switch (type)
        {
            case MessageType.ERROR:
            {
                textMessage.color = new Color(0.34f, 0.09f, 0.09f);
                image.sprite = backgrounds[0];
                break;
            }
            case MessageType.SUCCESS:
            {
                textMessage.color = new Color(0.09f, 0.27f, 0.09f);
                image.sprite = backgrounds[1];
                break;
            }
        }
    }
}