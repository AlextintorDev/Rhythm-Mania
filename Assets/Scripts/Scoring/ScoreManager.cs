using System;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    private int combo;
    private int score;
    [SerializeField] private GameUI gameUI;
    [SerializeField] private FeedbackController cameraShakeFeedback;


    void OnEnable()
    {
        GameEventBus.Subscribe<GameOverEvent>(OnGameOver);    
    }

    void OnDisable()
    {
        GameEventBus.Unsubscribe<GameOverEvent>(OnGameOver);    
    }

    void OnGameOver(GameOverEvent _)
    {
        Debug.Log("[ScoreManager] Game Over!");

        //Evitar nombres vacios
        if(string.IsNullOrEmpty(AnalyticsRecorder.GetPlayerName()))
            return;

        Debug.Log("[ScoreManager] Game Over! Final score: " + score);
        Leaderboard.AddScore(GameSettings.Instance.GetSong().songCode, AnalyticsRecorder.GetPlayerName(), score);
    }


    public void HitBeat(double difference, Note note)
    {
        GameSettings settings = GameSettings.Instance;

        Debug.Log("[ScoreManager] HitBeat with a difference: " + difference);

        //Ejemplo de sistema de puntuacion basado en la diferencia de tiempo, se puede ajustar segun sea necesario
        if (difference < settings.PerfectNoteThresholdMs)
        {
            //Perfect
            combo++;
            Debug.Log("[ScoreManager] Perfect hit!");
            score += 500;
            gameUI.UpdateScore(score, "¡Perfecto!");

            GameEventBus.Invoke(new HitEvent(HitType.Perfect, note));
            AnalyticsRecorder.RecordHit(AnalyticsRecorder.HitType.Perfect, difference);
        }
        else if (difference < settings.GoodNoteThresholdMs)
        {
            //Good
            combo++;
            Debug.Log("[ScoreManager] Good hit!");
            score += 250;
            gameUI.UpdateScore(score, "¡Genial!");

            GameEventBus.Invoke(new HitEvent(HitType.Good, note));
            AnalyticsRecorder.RecordHit(AnalyticsRecorder.HitType.Good, difference);
        }
        else
        {
            //Miss
            combo = 0;
            Debug.Log("[ScoreManager] Missed hit!");
            gameUI.UpdateScore(score, "¡Fallo!");

            if( cameraShakeFeedback != null)
                cameraShakeFeedback.PlayFeedback();

            GameEventBus.Invoke(new HitEvent(HitType.Miss, note));
            AnalyticsRecorder.RecordHit(AnalyticsRecorder.HitType.Miss, difference);
        }


        score = Mathf.Max(0,score - (int)difference);

        AnalyticsRecorder.RecordLevelScore(score);
        AnalyticsRecorder.RecordCombo(combo);
        gameUI.UpdateCombo(combo);
    }

}