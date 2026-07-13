using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSoundPlayer : MonoBehaviour
{
    private Button button;


    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    void OnEnable()
    {
        button.onClick.AddListener(PlayButtonSound);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(PlayButtonSound);
    }

    public static void PlayButtonSound()
    {
        MMSoundManagerSoundPlayEvent.Trigger(
                GameSettings.Instance.buttonSound,
                MMSoundManager.MMSoundManagerTracks.Sfx,
                Vector3.zero
            );
    }
}
