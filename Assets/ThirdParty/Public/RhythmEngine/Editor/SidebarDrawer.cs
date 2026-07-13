using RhythmEngine.Data;
using System;
using UnityEditor;
using UnityEngine;

namespace RhythmEngine.Editor
{
    internal sealed class SidebarDrawer
    {
        private readonly EditorState state;
        private readonly PlaybackController playbackController;
        private readonly Action<SongItem> setCurrentSong;
        private readonly Action createNewSongItem;
        private readonly Action saveSong;

        public SidebarDrawer(
            EditorState state,
            PlaybackController playbackController,
            Action<SongItem> setCurrentSong,
            Action createNewSongItem,
            Action saveSong)
        {
            this.state = state;
            this.playbackController = playbackController;
            this.setCurrentSong = setCurrentSong;
            this.createNewSongItem = createNewSongItem;
            this.saveSong = saveSong;
        }

        public void Draw()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(state.LeftPanelWidth));
            GUILayout.Label("Song Configuration", EditorStyles.boldLabel);

            SongItem newSong = (SongItem)EditorGUILayout.ObjectField(
                "Song Item",
                state.CurrentSong,
                typeof(SongItem),
                false);
            if (newSong != state.CurrentSong)
            {
                setCurrentSong(newSong);
            }

            if (state.CurrentSong == null)
            {
                EditorGUILayout.HelpBox("Select or create a Song Item.", MessageType.Info);
                if (GUILayout.Button("Create New Song Item"))
                {
                    createNewSongItem();
                }

                EditorGUILayout.EndVertical();
                return;
            }

            DrawProperties();
            DrawGridSettings();
            DrawChannels();
            DrawActions();
            DrawPreview();
            EditorGUILayout.EndVertical();
        }

        private void DrawProperties()
        {
            GUILayout.Space(8f);
            GUILayout.Label("Properties", EditorStyles.boldLabel);

            float newBpm = EditorGUILayout.FloatField("BPM", state.CurrentSong.bpm);
            if (newBpm != state.CurrentSong.bpm && newBpm > 0f)
            {
                Undo.RecordObject(state.CurrentSong, "Change BPM");
                state.CurrentSong.bpm = newBpm;
                state.MarkSongDirty();
            }

            float newOffset = EditorGUILayout.FloatField("Offset (s)", state.CurrentSong.firstBeatOffset);
            if (newOffset != state.CurrentSong.firstBeatOffset)
            {
                Undo.RecordObject(state.CurrentSong, "Change Offset");
                state.CurrentSong.firstBeatOffset = newOffset;
                state.MarkSongDirty();
            }
        }

        private void DrawGridSettings()
        {
            GUILayout.Space(8f);
            GUILayout.Label("Grid", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Division 1/", GUILayout.Width(70f));
            int maxDiv = state.EditorSettings != null ? Mathf.Max(1, state.EditorSettings.maxGridDivision) : 64;
            state.GridDivision = EditorGUILayout.IntSlider(state.GridDivision, 1, maxDiv);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawChannels()
        {
            GUILayout.Space(8f);
            GUILayout.Label("Channels", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Note Channel"))
            {
                Undo.RecordObject(state.CurrentSong, "Add Channel");
                state.CurrentSong.AddChannel($"Notes {state.CurrentSong.channels.Count}", ChannelType.Notes);
                state.MarkSongDirty();
            }

            if (GUILayout.Button("Add Waveform Channel"))
            {
                Undo.RecordObject(state.CurrentSong, "Add Channel");
                state.CurrentSong.AddChannel("Waveform", ChannelType.Waveform);
                state.MarkSongDirty();
            }
        }

        private void DrawActions()
        {
            GUILayout.Space(8f);
            GUILayout.Label("Actions", EditorStyles.boldLabel);
            if (GUILayout.Button("Clear All Notes") &&
                EditorUtility.DisplayDialog("Clear Notes", "Clear all notes?", "Yes", "Cancel"))
            {
                Undo.RecordObject(state.CurrentSong, "Clear All Notes");
                state.CurrentSong.ClearAllNotes();
                state.MarkSongDirty();
            }

            if (GUILayout.Button("Save Song"))
            {
                saveSong();
            }
        }

        private void DrawPreview()
        {
            GUILayout.Space(8f);
            GUILayout.Label("Preview", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(state.CurrentSong.clip == null);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(state.IsPlaying ? "Pause" : "Play"))
            {
                if (state.IsPlaying)
                {
                    playbackController.Pause();
                }
                else
                {
                    playbackController.Start();
                }
            }

            if (GUILayout.Button("Stop"))
            {
                playbackController.Stop();
            }

            EditorGUILayout.EndHorizontal();
            state.FollowPlayhead = EditorGUILayout.Toggle("Follow Playhead", state.FollowPlayhead);
            EditorGUILayout.LabelField(
                "Position",
                $"Beat {state.CurrentPlaybackBeat:F2} | {state.GetPlaybackTimeSeconds():F2}s");
            EditorGUI.EndDisabledGroup();
        }
    }
}
