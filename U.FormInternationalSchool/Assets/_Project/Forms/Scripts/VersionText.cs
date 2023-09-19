using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TextMeshProUGUI>().text = $"v{Application.version}";
    }
}
