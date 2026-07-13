using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private FeedbackController scoreFeedbackPlayer;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private FeedbackController comboFeedbackPlayer;
    [SerializeField] private Image EnergyBarFill;
    [SerializeField] private float fadeDuration = 1f;
    private float targetEnergyFill = 0f;
    private bool isUpdatingEnergyBar = false;

    [Header("GameOver")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    void Awake()
    {
        GameEventBus.Subscribe<GameOverEvent>(OnGameOver);
    }
    
    void OnDisable()
    {
        GameEventBus.Unsubscribe<GameOverEvent>(OnGameOver);
    }

    void OnGameOver(GameOverEvent gameOverEvent)
    {
        if(!gameOverEvent.win)
        {
            gameOverPanel.SetActive(true);
            FeedbackController feedback = gameOverPanel.GetComponent<FeedbackController>();
            if(feedback != null)
                feedback.PlayFeedback();
        }
        else
        {
            winPanel.SetActive(true);
            FeedbackController feedback = winPanel.GetComponent<FeedbackController>();
            if(feedback != null)
                feedback.PlayFeedback();
        }
    }

    void Update()
    {
        if(isUpdatingEnergyBar)
        {
            EnergyBarFill.fillAmount = Mathf.Lerp(EnergyBarFill.fillAmount, targetEnergyFill, Time.deltaTime / fadeDuration);

            if (Mathf.Abs(EnergyBarFill.fillAmount - targetEnergyFill) < 0.01f)
            {
                EnergyBarFill.fillAmount = targetEnergyFill;
                isUpdatingEnergyBar = false;
            }
        }
    }

    public void UpdateScore(int score, string windowText = "¡Perfect!")
    {
        if(scoreText == null) 
            return;
        scoreText.text = $"Score: {score}\n{windowText}";
        if(scoreFeedbackPlayer != null)
            scoreFeedbackPlayer.PlayFeedback();
    }

    public void UpdateCombo(int combo)
    {
        comboText.text = $"Combo: {combo}";
        if(comboFeedbackPlayer != null)
            comboFeedbackPlayer.PlayFeedback();
    }

    public void UpdateEnergyBar(float energyPercentage)
    {
        targetEnergyFill = energyPercentage;
        if(GameSettings.Instance.UsingGameFeel)
        {
            isUpdatingEnergyBar = true;
        }
        else
        {
            EnergyBarFill.fillAmount = targetEnergyFill;
        }
    }
}