using UnityEngine;
using UnityEditor;
using RhythmEngine.Data;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NoteData = RhythmEngine.Data.NoteData;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace RhythmEngine.Editor
{
    public static class MidiImporter
    {
        public static void ImportMidiToSongItem(string filePath, SongItem songItem, float quantizeStep)
        {
            if (songItem == null)
            {
                throw new System.ArgumentNullException("Song item cannot be null");
            }

            if (!File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException($"MIDI file not found: {filePath}");
            }

            Debug.Log($"Importing MIDI file: {Path.GetFileName(filePath)}");

            try
            {
                // Read MIDI file
                var midiFile = MidiFile.Read(filePath);
                var tempoMap = midiFile.GetTempoMap();

                // Extract BPM from tempo map
                var firstTempo = tempoMap.GetTempoAtTime(new MetricTimeSpan(0));
                float bpm = (float)firstTempo.BeatsPerMinute;

                if (bpm > 0 && bpm < 500) // Reasonable BPM range
                {
                    songItem.bpm = bpm;
                    Debug.Log($"Detected BPM: {bpm}");
                }
                else
                {
                    Debug.LogWarning($"Invalid BPM detected ({bpm}), keeping current BPM: {songItem.bpm}");
                }

                // Get all notes from MIDI file
                var notes = midiFile.GetNotes();

                if (notes.Count == 0)
                {
                    Debug.LogWarning("No notes found in MIDI file");
                    return;
                }

                // Group notes by channel/track
                var notesByChannel = notes.GroupBy(n => n.Channel).OrderBy(g => g.Key).ToList();

                // Clear existing note channels
                for (int i = songItem.channels.Count - 1; i >= 0; i--)
                {
                    if (songItem.channels[i].type == ChannelType.Notes)
                    {
                        songItem.RemoveChannel(i);
                    }
                }

                // Process each MIDI channel as a separate note channel
                for (int channelIndex = 0; channelIndex < notesByChannel.Count; channelIndex++)
                {
                    var channelNotes = notesByChannel[channelIndex];
                    string channelName = $"MIDI Ch {channelNotes.Key + 1}";
                    
                    // Add new note channel
                    songItem.AddChannel(channelName, ChannelType.Notes);
                    var channel = songItem.channels[songItem.channels.Count - 1];

                    foreach (var midiNote in channelNotes)
                    {
                        // Convert MIDI time to beats/seconds
                        var metricTime = TimeConverter.ConvertTo<MetricTimeSpan>(midiNote.Time, tempoMap);
                        float timeInSeconds = (float)metricTime.TotalMicroseconds / 1000000f;
                        float beat = songItem.SecondsToBeats(timeInSeconds);

                        // Quantize to grid
                        float quantizedBeat = songItem.QuantizeBeat(beat, quantizeStep);
                        float quantizedTime = songItem.BeatsToSeconds(quantizedBeat);

                        // Calculate note length
                        var metricLength = LengthConverter.ConvertTo<MetricTimeSpan>(midiNote.Length, midiNote.Time, tempoMap);
                        float lengthInSeconds = (float)metricLength.TotalMicroseconds / 1000000f;
                        float lengthInBeats = lengthInSeconds / (60f / songItem.bpm);

                        // Check if note already exists at this beat
                        var existingNote = channel.trackData.GetNoteAt(quantizedBeat);
                        if (existingNote == null)
                        {
                            // Create new note
                            var noteData = new NoteData(quantizedBeat, lengthInBeats);
                            noteData.timeSeconds = quantizedTime;
                            channel.trackData.AddNote(noteData);
                        }
                    }

                    Debug.Log($"Channel {channelIndex + 1}: Imported {channel.trackData.notes.Count} notes from MIDI channel {channelNotes.Key + 1}");
                }

                Debug.Log($"MIDI import completed: {notesByChannel.Count} channels, BPM: {songItem.bpm}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing MIDI file: {e.Message}");
                throw;
            }
        }
        
        [MenuItem("Assets/Import MIDI to Song", false, 30)]
        private static void ImportMidiAsset()
        {
            string[] guids = Selection.assetGUIDs;
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                
                if (Path.GetExtension(path).ToLower() == ".mid")
                {
                    string songPath = Path.ChangeExtension(path, ".asset");
                    
                    SongItem newSong = ScriptableObject.CreateInstance<SongItem>();
                    newSong.name = Path.GetFileNameWithoutExtension(path);
                    
                    try
                    {
                        ImportMidiToSongItem(path, newSong, 0.25f);
                        
                        AssetDatabase.CreateAsset(newSong, songPath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        
                        Debug.Log($"Created song asset: {songPath}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Failed to import MIDI {path}: {e.Message}");
                        ScriptableObject.DestroyImmediate(newSong);
                    }
                }
            }
        }
        
        [MenuItem("Assets/Import MIDI to Song", true)]
        private static bool ImportMidiAssetValidate()
        {
            string[] guids = Selection.assetGUIDs;
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetExtension(path).ToLower() == ".mid")
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}