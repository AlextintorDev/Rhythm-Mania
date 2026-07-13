using System.Collections.Generic;
using UnityEngine;

public static class AnalyticsRecorder
{
    private static SessionData sessionData;
    private static int currentLevelIndex = -1;
    private static TryData currentTry;

    public static void Initialize()
    {
        sessionData = new SessionData
        {
            gameFeelActive = GameSettings.Instance.UsingGameFeel,
            levels = new List<LevelData>()
        };
    }

    public static void SetPlayerName(string name)
    {
        sessionData.playerName = name;
    }

    public static string GetPlayerName()
    {
        return sessionData.playerName;
    }

    public static void StartNewTry(int levelIndex)
    {
        currentLevelIndex = levelIndex;

        LevelData levelData = sessionData.levels.Find(l => l.levelIndex == levelIndex);
        if (levelData == null)
        {
            levelData = new LevelData { levelIndex = levelIndex, tries = new List<TryData>() };
            sessionData.levels.Add(levelData);
        }

        currentTry = new TryData { hits = new List<HitData>() };
        levelData.tries.Add(currentTry);
    }

    public static void RecordLevelScore(int score)
    {
        currentTry.score = score;
    }

    public static void RecordHit(HitType hitType, double difference)
    {
        currentTry.hits.Add(new HitData
        {
            hitType = hitType.ToString(),
            differenceMs = difference * 1000.0
        });
    }

    public static void RecordGhostNote()
    {
        currentTry.ghostNotes++;
    }

    public static void RecordCombo(int combo)
    {
        if (combo > currentTry.maxCombo)
            currentTry.maxCombo = combo;
    }

    public static string GetShareableCode()
    {
        return JsonUtility.ToJson(sessionData);
    }

    public static string ToJson()
    {
        return JsonUtility.ToJson(sessionData, true);
    }

    public static string GetCurrentLevelDataSerialized()
    {
        LevelData levelData = sessionData.levels.Find(l => l.levelIndex == currentLevelIndex);
        return JsonUtility.ToJson(levelData, true);
    }

    public enum HitType
    {
        Perfect,
        Good,
        Miss
    }

    [System.Serializable]
    private class SessionData
    {
        public string playerName;
        public bool gameFeelActive;
        public List<LevelData> levels;
    }

    [System.Serializable]
    private class LevelData
    {
        public int levelIndex;
        public List<TryData> tries;
    }

    [System.Serializable]
    private class TryData
    {
        public int score;
        public int maxCombo;
        public int ghostNotes;
        public List<HitData> hits;
    }

    [System.Serializable]
    private class HitData
    {
        public string hitType;
        public double differenceMs;
    }
}
