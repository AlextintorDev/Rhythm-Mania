using TMPro;
using UnityEngine;

public class NameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    private bool active = true;
    public void SetName()
    {
        if(!active) 
            return;
        string newName = inputField.text;
        newName = newName.Replace(" ", "_");
        if (string.IsNullOrEmpty(newName))
            return;
        active = false;
        AnalyticsRecorder.SetPlayerName(newName);
        GameManager.Instance.GoToScene("MainMenu");
    }
}
