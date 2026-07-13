using RhythmEngine.Data;
using UnityEditor;
using UnityEngine;

namespace RhythmEngine.Editor
{
    public class SongEditorWindow : EditorWindow
    {
        private EditorState state;
        private PlaybackController playbackController;
        private NoteRenderer noteRenderer;
        private WaveformRenderer waveformRenderer;
        private GridRenderer gridRenderer;
        private GridInputHandler inputHandler;
        private SidebarDrawer sidebarDrawer;

        [MenuItem("Window/Rhythm Sequencer")]
        public static void ShowWindow()
        {
            SongEditorWindow window = GetWindow<SongEditorWindow>("Rhythm Sequencer");
            window.minSize = new Vector2(900f, 600f);
            window.Show();
        }

        private void OnEnable()
        {
            state = new EditorState(this);
            state.LoadEditorSettings();

            playbackController = new PlaybackController(state);
            playbackController.Initialize();

            noteRenderer = new NoteRenderer(state);
            waveformRenderer = new WaveformRenderer(state);
            gridRenderer = new GridRenderer(state, noteRenderer, waveformRenderer);
            inputHandler = new GridInputHandler(state, noteRenderer, playbackController);
            sidebarDrawer = new SidebarDrawer(state, playbackController, SetCurrentSong, CreateNewSongItem, SaveSong);
        }

        private void OnDisable()
        {
            playbackController?.Dispose();
            waveformRenderer?.Dispose();
        }

        private void OnGUI()
        {
            if (inputHandler != null)
            {
                inputHandler.HandleKeyboardShortcuts();
            }

            EditorGUILayout.BeginHorizontal();
            if (sidebarDrawer != null)
            {
                sidebarDrawer.Draw();
            }

            EditorGUILayout.BeginVertical();
            DrawToolbar();
            if (gridRenderer != null)
            {
                gridRenderer.DrawMainGrid(inputHandler != null ? inputHandler.HandleInput : null);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if (state != null && state.IsPlaying)
            {
                playbackController.Update();
                state.RequestRepaint();
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label("Zoom", GUILayout.Width(40f));
            state.PixelsPerSecond = GUILayout.HorizontalSlider(
                state.PixelsPerSecond,
                state.MinPixelsPerSecond,
                state.MaxPixelsPerSecond,
                GUILayout.Width(100f));

            state.SnapToGrid = GUILayout.Toggle(state.SnapToGrid, "Snap", EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("?", EditorStyles.toolbarButton, GUILayout.Width(25f)))
            {
                OpenEditorSettings();
            }

            EditorGUI.BeginDisabledGroup(state.CurrentSong == null);
            if (GUILayout.Button("Import MIDI", EditorStyles.toolbarButton))
            {
                ImportMidiFile();
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Reset View", EditorStyles.toolbarButton))
            {
                state.GridScrollPos = Vector2.zero;
                state.PixelsPerSecond = 100f;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OpenEditorSettings()
        {
            if (state.EditorSettings != null)
            {
                Selection.activeObject = state.EditorSettings;
                EditorGUIUtility.PingObject(state.EditorSettings);
                return;
            }

            EditorUtility.DisplayDialog(
                "Settings Not Found",
                "SongEditorSettings asset not found in Resources folder.\n\nIt will be created automatically on next window open.",
                "OK");
        }

        private void ImportMidiFile()
        {
            if (state.CurrentSong == null)
            {
                return;
            }

            string path = EditorUtility.OpenFilePanel("Select MIDI File", string.Empty, "mid");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                Undo.RecordObject(state.CurrentSong, "Import MIDI");
                MidiImporter.ImportMidiToSongItem(path, state.CurrentSong, 1f / state.GridDivision);
                state.MarkSongDirty();
                EditorUtility.DisplayDialog("MIDI Import", "MIDI file imported successfully!", "OK");
            }
            catch (System.Exception exception)
            {
                EditorUtility.DisplayDialog(
                    "Import Error",
                    $"Failed to import MIDI file:\n{exception.Message}",
                    "OK");
                Debug.LogError($"MIDI import error: {exception}");
            }
        }

        private void SetCurrentSong(SongItem song)
        {
            state.CurrentSong = song;
            if (state.CurrentSong != null)
            {
                state.SerializedSong = new SerializedObject(state.CurrentSong);
            }
            else
            {
                state.SerializedSong = null;
            }

            playbackController?.SyncCurrentSongClip();
        }

        private void CreateNewSongItem()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Song Item",
                "NewSong",
                "asset",
                "Enter a name for the new song item");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            SongItem newSong = CreateInstance<SongItem>();
            AssetDatabase.CreateAsset(newSong, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            SetCurrentSong(newSong);
        }

        private void SaveSong()
        {
            if (state.CurrentSong == null)
            {
                return;
            }

            EditorUtility.SetDirty(state.CurrentSong);
            AssetDatabase.SaveAssets();
        }
    }
}