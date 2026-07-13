using System;
using GameSaveSystem.Core;
using RhythmEngine.Data;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Rhythm Mania/Game Settings")]
public class GameSettings : ScriptableObject
{
    public static GameSettings instance;
    public static GameSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameSettings>("GameSettings");
                if (instance == null)
                {
                    Debug.LogError("GameSettings asset not found in Resources folder! Creating a new one.");
                    instance = CreateInstance<GameSettings>();
                }
            }
            return instance;
        }
    }

    [Header("Gameplay Settings")]
    [SerializeField] private bool usingGameFeel = false;
    public bool UsingGameFeel => usingGameFeel;
    [SerializeField] private float noteSpeed = 3f;
    public float NoteSpeed => noteSpeed;
    [SerializeField] private double firstBeatOffset = 2f;
    public double FirstBeatOffset => firstBeatOffset;
    [SerializeField] private Vector3 particleOffset = Vector3.zero;
    public Vector3 ParticleOffset => particleOffset;
    [SerializeField] private double perfectNoteThresholdMs = 80d;
    public double PerfectNoteThresholdMs => perfectNoteThresholdMs;
    [SerializeField] private double goodNoteThresholdMs = 100d;
    public double GoodNoteThresholdMs => goodNoteThresholdMs;
    [SerializeField] private SongItem[] songs;
    [SerializeField] private int index = 0;

    [Header("UI")]
    public AudioClip buttonSound;
    public int SongIndex 
    {
        get => index; 

        set 
        {
        index = Math.Clamp(value, 0, songs.Length - 1);
        }
    }
    public SongItem GetSong() => songs[index];

    [Header("Debug Settings")]
    [SerializeField] private bool recording = false;
    public bool Recording => recording;

    public void SetGameFeel(bool value)
    {
        usingGameFeel = value;
    }

    public void SetSongIndex(SongItem song)
    {
        int newIndex = Array.IndexOf(songs, song);
        if (newIndex != -1)
        {
            SongIndex = newIndex;
        }
        else
        {
            Debug.LogWarning("Song not found in the list of songs.");
        }
    }
}

