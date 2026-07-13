using RhythmEngine.Data;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace RhythmEngine.Editor
{
    public class NoteEditorWindow : EditorWindow
    {
        private SongItem song;
        private int channelIndex;
        private NoteData note;

        public static void Open(SongItem song, int channelIndex, NoteData note)
        {
            var window = GetWindow<NoteEditorWindow>(true, "Note Editor", true);
            window.song = song;
            window.channelIndex = channelIndex;
            window.note = note;
            window.minSize = new Vector2(360, 320);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            if (song == null || note == null)
            {
                EditorGUILayout.HelpBox("No note selected.", MessageType.Info);
                if (GUILayout.Button("Close")) this.Close();
                return;
            }

            GUILayout.Label("Time/Beat", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            float newTime = EditorGUILayout.FloatField("Time (s)", note.timeSeconds);
            float newBeat = EditorGUILayout.FloatField("Beat", note.beat);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(song, "Edit Note Time/Beat");
                if (!Mathf.Approximately(newTime, note.timeSeconds))
                {
                    note.timeSeconds = Mathf.Max(0f, newTime);
                    note.beat = song.SecondsToBeats(note.timeSeconds);
                }
                else if (!Mathf.Approximately(newBeat, note.beat))
                {
                    note.beat = Mathf.Max(0f, newBeat);
                    note.timeSeconds = song.BeatsToSeconds(note.beat);
                }
                MarkSongDirty();
            }

            GUILayout.Space(6);
            GUILayout.Label("Note Properties", EditorStyles.boldLabel);
            DrawNoteFields(note);

            GUILayout.Space(6);
            GUILayout.Label("Payload", EditorStyles.boldLabel);
            if (note.payload == null) note.payload = new NotePayload();
            DrawObjectPublicFields(note.payload);

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete"))
            {
                if (channelIndex >= 0 && channelIndex < song.channels.Count)
                {
                    var channel = song.channels[channelIndex];
                    if (channel.type == ChannelType.Notes && channel.trackData != null)
                    {
                        Undo.RecordObject(song, "Delete Note");
                        channel.trackData.RemoveNoteAt(note.beat, 0.001f);
                        MarkSongDirty();
                    }
                }
                Close();
            }
            if (GUILayout.Button("Close"))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawNoteFields(NoteData selectedNote)
        {
            var noteType = typeof(NoteData);
            var fields = noteType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fi in fields)
            {
                if (fi.Name == nameof(NoteData.beat) || fi.Name == nameof(NoteData.timeSeconds) || fi.Name == nameof(NoteData.payload)) continue;
                var fType = fi.FieldType;
                object value = fi.GetValue(selectedNote);
                string label = ObjectNames.NicifyVariableName(fi.Name);

                EditorGUI.BeginChangeCheck();
                if (fType == typeof(int))
                {
                    int v = EditorGUILayout.IntField(label, value != null ? (int)value : 0);
                    if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(song, "Edit Note Field"); fi.SetValue(selectedNote, v); MarkSongDirty(); }
                }
                else if (fType == typeof(float))
                {
                    float v = EditorGUILayout.FloatField(label, value != null ? (float)value : 0f);
                    if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(song, "Edit Note Field"); fi.SetValue(selectedNote, v); MarkSongDirty(); }
                }
                else if (fType == typeof(bool))
                {
                    bool v = EditorGUILayout.Toggle(label, value != null ? (bool)value : false);
                    if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(song, "Edit Note Field"); fi.SetValue(selectedNote, v); MarkSongDirty(); }
                }
                else if (fType == typeof(string))
                {
                    string v = EditorGUILayout.TextField(label, value != null ? (string)value : string.Empty);
                    if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(song, "Edit Note Field"); fi.SetValue(selectedNote, v); MarkSongDirty(); }
                }
                else if (fType.IsEnum)
                {
                    System.Enum v = (System.Enum)(value ?? System.Enum.GetValues(fType).GetValue(0));
                    var nv = EditorGUILayout.EnumPopup(label, v);
                    if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(song, "Edit Note Field"); fi.SetValue(selectedNote, nv); MarkSongDirty(); }
                }
                else
                {
                    EditorGUILayout.LabelField(label, value != null ? value.ToString() : "null");
                    EditorGUI.EndChangeCheck();
                }
            }
        }

        private void DrawObjectPublicFields(object target)
        {
            if (target == null) return;
            var type = target.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fi in fields)
            {
                var fType = fi.FieldType;
                object value = fi.GetValue(target);
                string label = ObjectNames.NicifyVariableName(fi.Name);

                EditorGUI.BeginChangeCheck();
                if (fType == typeof(int))
                {
                    int v = EditorGUILayout.IntField(label, value != null ? (int)value : 0);
                    if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(song, "Edit Note"); fi.SetValue(target, v); MarkSongDirty(); }
                }
                else if (fType == typeof(float))
                {
                    float v = EditorGUILayout.FloatField(label, value != null ? (float)value : 0f);
                    if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(song, "Edit Note"); fi.SetValue(target, v); MarkSongDirty(); }
                }
                else if (fType == typeof(bool))
                {
                    bool v = EditorGUILayout.Toggle(label, value != null ? (bool)value : false);
                    if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(song, "Edit Note"); fi.SetValue(target, v); MarkSongDirty(); }
                }
                else if (fType == typeof(string))
                {
                    string v = EditorGUILayout.TextField(label, value != null ? (string)value : string.Empty);
                    if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(song, "Edit Note"); fi.SetValue(target, v); MarkSongDirty(); }
                }
                else if (fType.IsEnum)
                {
                    System.Enum v = (System.Enum)(value ?? System.Enum.GetValues(fType).GetValue(0));
                    var nv = EditorGUILayout.EnumPopup(label, v);
                    if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(song, "Edit Note"); fi.SetValue(target, nv); MarkSongDirty(); }
                }
                else
                {
                    // Unsupported complex field: show read-only label
                    EditorGUILayout.LabelField(label, value != null ? value.ToString() : "null");
                    EditorGUI.EndChangeCheck();
                }
            }
        }

        private void MarkSongDirty()
        {
            if (song != null)
            {
                EditorUtility.SetDirty(song);
            }
        }
    }
}