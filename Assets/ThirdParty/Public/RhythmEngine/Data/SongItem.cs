using System.Collections.Generic;
using UnityEngine;

namespace RhythmEngine.Data
{
    [CreateAssetMenu(fileName = "New Song", menuName = "Rhythm/Song Item")]
    public class SongItem : ScriptableObject
    {
        [Header("Song Settings")]
        public string songName;
        public string songCode;
        public AudioClip clip;
        public float bpm = 120f;
        public float firstBeatOffset = 0f; // Seconds
        
        [Header("Channel Configuration")]
        public List<ChannelData> channels = new List<ChannelData>();
        
        // Keep tracks for migration purposes, but it won't be exposed in the inspector
        // after migration, new systems should use 'channels'.
        // If you want to completely remove it after migration, you'd need a custom editor
        // or a post-processor to clear it. For now, it remains for data integrity.
        [SerializeField] private List<TrackData> tracks = new List<TrackData>();

        private void OnEnable()
        {
            // Migration: Convert legacy tracks to channels if needed
            if (tracks.Count > 0 && channels.Count == 0)
            {
                foreach (var track in tracks)
                {
                    var channel = new ChannelData(track.tag, ChannelType.Notes);
                    channel.trackData = track; // Preserve existing data
                    channels.Add(channel);
                }
                // We don't clear tracks immediately to be safe, but we could.
                // For now, let's keep them in sync or just use channels moving forward.
            }

            // Ensure we have at least one channel if completely empty
            if (channels.Count == 0)
            {
                AddChannel("Waveform", ChannelType.Waveform);
                AddChannel("Notes 1", ChannelType.Notes);
            }
        }
        
        public void AddChannel(string name, ChannelType type)
        {
            channels.Add(new ChannelData(name, type));
        }
        
        public void RemoveChannel(int index)
        {
            if (index >= 0 && index < channels.Count)
            {
                channels.RemoveAt(index);
            }
        }
        
        // Legacy support / Helper for Note channels
        public void ToggleNoteAt(int channelIndex, float beat)
        {
            if (channelIndex < 0 || channelIndex >= channels.Count) return;
            
            var channel = channels[channelIndex];
            if (channel.type != ChannelType.Notes || channel.trackData == null) return;
            
            NoteData existingNote = channel.trackData.GetNoteAt(beat);
            
            if (existingNote != null)
            {
                channel.trackData.RemoveNoteAt(beat);
            }
            else
            {
                channel.trackData.AddNote(new NoteData(beat));
            }
        }
        
        public NoteData GetNoteAt(int channelIndex, float beat)
        {
            if (channelIndex < 0 || channelIndex >= channels.Count) return null;
            var channel = channels[channelIndex];
            if (channel.type != ChannelType.Notes || channel.trackData == null) return null;
            
            return channel.trackData.GetNoteAt(beat);
        }
        
        public bool HasNoteAt(int channelIndex, float beat)
        {
            return GetNoteAt(channelIndex, beat) != null;
        }
        
        public void ClearAllNotes()
        {
            foreach (var channel in channels)
            {
                if (channel.type == ChannelType.Notes && channel.trackData != null)
                {
                    channel.trackData.ClearNotes();
                }
            }
        }
        
        // Helper methods for time conversion
        public float BeatsToSeconds(float beats)
        {
            float secondsPerBeat = 60f / bpm;
            return firstBeatOffset + beats * secondsPerBeat;
        }
        
        public float SecondsToBeats(float seconds)
        {
            float secondsPerBeat = 60f / bpm;
            return (seconds - firstBeatOffset) / secondsPerBeat;
        }
        
        public float QuantizeBeat(float beat, float step)
        {
            return Mathf.Round(beat / step) * step;
        }
        
        public float GetSongLengthInBeats()
        {
            if (clip == null) return 32f; // Default length
            
            float totalSeconds = clip.length;
            return SecondsToBeats(totalSeconds);
        }
    }
}