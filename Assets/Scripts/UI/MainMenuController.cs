using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectionPanel;

    void Awake()
    {
        string fgs = Metadata.GetMetadata("fromGameScene", true);
        if(!string.IsNullOrEmpty(fgs) && bool.TryParse(fgs, out bool r) && r)
        {
            ShowLevelSelection();
        }
    }

    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void HideCredits()
    {
        creditsPanel.SetActive(false);
    }

    public void ShowLevelSelection()
    {
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
    }

    public void HideLevelSelection()
    {
        mainMenuPanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
