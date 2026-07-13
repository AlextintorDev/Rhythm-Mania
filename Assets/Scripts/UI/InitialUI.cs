using System.Collections;
using TMPro;
using UnityEngine;

public class InitialUI : MonoBehaviour
{
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject comboText;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private TMP_Text songNameText;
    [SerializeField] private TMP_Text songNamePauseText;

    [SerializeField] private CanvasGroup songNameCanvasGroup;

    void Awake()
    {
        scoreText.SetActive(false);
        comboText.SetActive(false);
        tutorialPanel.SetActive(true);
        songNamePauseText.text = GameSettings.Instance.GetSong().songName;
    }

    void Update()
    {
        if(IsStartPressed())
        {
            StartGame();
            StartCoroutine(ShowSongName());
            enabled = false;
        }
    }

    bool IsStartPressed()
    {
        return 
        Input.GetKeyDown(KeyCode.Space);
    }

    void StartGame()
    {
        scoreText.SetActive(true);
        comboText.SetActive(true);
        tutorialPanel.SetActive(false);
        levelManager.StartLevel();
    }

    IEnumerator ShowSongName()
    {
        double beatOffset = GameSettings.instance.FirstBeatOffset;
        double transitionDuration = beatOffset * 0.2d;
        double displayDuration = beatOffset * 0.8d;

        double elapsed = 0d;


        songNameText.text = GameSettings.Instance.GetSong().songName;
        SetSongNameAlpha(0f);
        songNameCanvasGroup.gameObject.SetActive(true);

        while (elapsed < transitionDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, (float)(elapsed / transitionDuration));
            SetSongNameAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetSongNameAlpha(1f);
        yield return new WaitForSeconds((float)displayDuration);

        elapsed = 0d;
        while (elapsed < transitionDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, (float)(elapsed / transitionDuration));
            SetSongNameAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        songNameCanvasGroup.gameObject.SetActive(false);
    }

    void SetSongNameAlpha(float alpha)
    {
        songNameCanvasGroup.alpha = alpha;
    }

}
