using RhythmEngine.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RhythmEngine.Editor
{
    internal sealed class EditorState
    {
        private readonly EditorWindow owner;

        public EditorState(EditorWindow owner)
        {
            this.owner = owner;
        }

        public SongItem CurrentSong;
        public SerializedObject SerializedSong;
        public SongEditorSettings EditorSettings;

        public float LeftPanelWidth = 260f;
        public float ChannelHeaderWidth = 150f;
        public float TimelineHeight = 30f;
        public Vector2 GridScrollPos;

        public float PixelsPerSecond = 100f;
        public float MinPixelsPerSecond = 10f;
        public float MaxPixelsPerSecond = 1000f;

        public int GridDivision = 4;
        public bool SnapToGrid = true;

        public bool IsPlaying;
        public bool IsPaused;
        public bool FollowPlayhead = true;
        public float CurrentPlaybackBeat;
        public float PreviousPlaybackBeat;
        public AudioSource PreviewAudioSource;
        public AudioSource PreviewNoteSound;
        public readonly HashSet<string> TriggeredNotesThisFrame = new HashSet<string>();

        public NoteData DraggingNote;
        public int DraggingChannelIndex = -1;
        public float DragStartBeat;
        public Vector2 DragStartMouse;
        public bool IsDraggingNote;

        public const float NoteHitRadius = 10f;
        public const float MinSubdivisionPixelSpacing = 6f;
        public const float MinBeatLabelSpacing = 45f;

        public readonly Color GridBackgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);
        public readonly Color MajorLine = new Color(1f, 1f, 1f, 0.2f);
        public readonly Color MinorLine = new Color(1f, 1f, 1f, 0.05f);
        public readonly Color ChannelSeparator = new Color(0f, 0f, 0f, 0.5f);

        public Rect WindowRect => owner != null ? owner.position : default;

        public void LoadEditorSettings()
        {
            EditorSettings = Resources.Load<SongEditorSettings>("SongEditorSettings");
            if (EditorSettings != null)
            {
                return;
            }

            SongEditorSettings settings = ScriptableObject.CreateInstance<SongEditorSettings>();
            string resourcesDir = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder("Assets"))
            {
                AssetDatabase.CreateFolder(string.Empty, "Assets");
            }

            if (!AssetDatabase.IsValidFolder(resourcesDir))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            string assetPath = "Assets/Resources/SongEditorSettings.asset";
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSettings = settings;
        }

        public void MarkSongDirty()
        {
            if (CurrentSong != null)
            {
                EditorUtility.SetDirty(CurrentSong);
            }
        }

        public void RequestRepaint()
        {
            owner?.Repaint();
        }

        public float GetSongLengthSeconds()
        {
            if (CurrentSong == null)
            {
                return 0f;
            }

            if (CurrentSong.clip != null)
            {
                return CurrentSong.clip.length;
            }

            return CurrentSong.BeatsToSeconds(CurrentSong.GetSongLengthInBeats());
        }

        public float GetSongLengthBeats()
        {
            if (CurrentSong == null)
            {
                return 0f;
            }

            if (CurrentSong.clip != null)
            {
                return Mathf.Max(0f, CurrentSong.SecondsToBeats(CurrentSong.clip.length));
            }

            return Mathf.Max(0f, CurrentSong.GetSongLengthInBeats());
        }

        public float GetPlaybackTimeSeconds()
        {
            if (PreviewAudioSource != null && CurrentSong?.clip != null)
            {
                return PreviewAudioSource.time;
            }

            if (CurrentSong == null)
            {
                return 0f;
            }

            return Mathf.Max(0f, CurrentSong.BeatsToSeconds(CurrentPlaybackBeat));
        }

        public float GetNoteVisualSize()
        {
            if (CurrentSong == null || CurrentSong.bpm <= 0f)
            {
                return 20f;
            }

            float beatDuration = 60f / CurrentSong.bpm;
            return Mathf.Clamp(beatDuration * PixelsPerSecond * 0.35f, 8f, 22f);
        }
    }
}
