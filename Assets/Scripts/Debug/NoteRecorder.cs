using System.Collections.Generic;
using RhythmEngine.Data;
using UnityEngine;

public class NoteRecorder : MonoBehaviour
{
    [SerializeField] private BeatScroller beatScroller;
    [SerializeField] private InputKey rail1InputKey;
    [SerializeField] private InputKey rail2InputKey;
    [SerializeField] private InputKey rail3InputKey;
    [SerializeField] private InputKey rail4InputKey;

    [Header("Debug")]
    [SerializeField] private List<KeyCode> rail1Keys;
    [SerializeField] private List<KeyCode> rail2Keys;
    [SerializeField] private List<KeyCode> rail3Keys;
    [SerializeField] private List<KeyCode> rail4Keys;

    [SerializeField] private TrackData rail1Track;
    [SerializeField] private TrackData rail2Track;
    [SerializeField] private TrackData rail3Track;
    [SerializeField] private TrackData rail4Track;

    void Awake()
    {
        if(!GameSettings.Instance.Recording)
        {
            enabled = false;
            return;
        }

        SongItem song = GameSettings.Instance.GetSong();
        rail1Track = song.channels[1].trackData;
        rail2Track = song.channels[2].trackData;
        rail3Track = song.channels[3].trackData;
        rail4Track = song.channels[4].trackData;

        rail1Keys = new List<KeyCode>(rail1InputKey.keys);
        rail2Keys = new List<KeyCode>(rail2InputKey.keys);
        rail3Keys = new List<KeyCode>(rail3InputKey.keys);
        rail4Keys = new List<KeyCode>(rail4InputKey.keys);
    }

    void Update()
    {
        CheckInputForRail(rail1Keys, rail1Track);
        CheckInputForRail(rail2Keys, rail2Track);
        CheckInputForRail(rail3Keys, rail3Track);
        CheckInputForRail(rail4Keys, rail4Track);
    }

    private void CheckInputForRail(List<KeyCode> keys, TrackData track)
    {
        foreach (var key in keys)
        {
            if(Input.GetKeyDown(key))
            {
                float currentBeat = beatScroller.GetCurrentBeat();
                track.AddNote(new NoteData(currentBeat));
                Debug.Log($"Added note at beat {currentBeat} to track {track.tag}");
            }
        }
    }

}
