using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    [SerializeField] TMP_Text versionText;
    void Awake()
    {
        if(versionText == null)
            versionText = GetComponent<TMP_Text>();

        versionText.text = $"Hecho por Alexander Correa\nRhythm Mania v{Application.version}{GameSettings.Instance.UsingGameFeel}";
    }
}
