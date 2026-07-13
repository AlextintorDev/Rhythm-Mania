using RhythmEngine.Data;
using UnityEditor;
using UnityEngine;

namespace RhythmEngine.Editor
{
    internal sealed class NoteRenderer
    {
        private readonly EditorState state;

        public NoteRenderer(EditorState state)
        {
            this.state = state;
        }

        public void DrawNotes(Rect rect, ChannelData channel)
        {
            if (channel.trackData == null || state.CurrentSong == null)
            {
                return;
            }

            Event currentEvent = Event.current;
            Vector2 mousePos = currentEvent.mousePosition;

            foreach (NoteData note in channel.trackData.notes)
            {
                float time = state.CurrentSong.BeatsToSeconds(note.beat);
                float x = rect.x + time * state.PixelsPerSecond;
                float noteSize = state.GetNoteVisualSize();
                float y = rect.y + (rect.height - noteSize) / 2f;
                Rect noteRect = new Rect(x - noteSize * 0.5f, y, noteSize, noteSize);

                if (x < state.GridScrollPos.x + state.ChannelHeaderWidth - noteSize ||
                    x > state.GridScrollPos.x + state.WindowRect.width + noteSize)
                {
                    continue;
                }

                Color noteColor = GetNoteColor(note);
                DrawNote(noteRect, noteColor);
                DrawTooltip(noteRect, note, mousePos);
            }
        }

        public NoteData FindNoteAtPosition(ChannelData channel, Vector2 mousePos, Rect contentRect, int channelIndex)
        {
            if (channel.trackData == null || state.CurrentSong == null)
            {
                return null;
            }

            float channelY = contentRect.y + state.TimelineHeight;
            for (int i = 0; i < channelIndex; i++)
            {
                channelY += state.CurrentSong.channels[i].height;
            }

            foreach (NoteData note in channel.trackData.notes)
            {
                float time = state.CurrentSong.BeatsToSeconds(note.beat);
                float x = contentRect.x + state.ChannelHeaderWidth + time * state.PixelsPerSecond;
                float noteSize = state.GetNoteVisualSize();
                float y = channelY + (channel.height - noteSize) / 2f;
                Rect noteRect = new Rect(x - noteSize * 0.5f, y, noteSize, noteSize);

                if (noteRect.Contains(mousePos))
                {
                    return note;
                }
            }

            return null;
        }

        private Color GetNoteColor(NoteData note)
        {
            if (state.IsDraggingNote && state.DraggingNote == note)
            {
                return Color.yellow;
            }

            if (state.EditorSettings != null && state.EditorSettings.beatIconTint != Color.clear)
            {
                return state.EditorSettings.beatIconTint;
            }

            return Color.green;
        }

        private void DrawNote(Rect noteRect, Color noteColor)
        {
            if (state.EditorSettings != null && state.EditorSettings.beatIcon != null)
            {
                Color oldColor = GUI.color;
                GUI.color = noteColor;
                GUI.DrawTexture(noteRect, state.EditorSettings.beatIcon);
                GUI.color = oldColor;
                return;
            }

            EditorGUI.DrawRect(noteRect, noteColor);
        }

        private void DrawTooltip(Rect noteRect, NoteData note, Vector2 mousePos)
        {
            if (state.EditorSettings == null ||
                !state.EditorSettings.showTooltips ||
                !noteRect.Contains(mousePos))
            {
                return;
            }

            string tooltip = $"Beat: {note.beat:F2}\nTime: {note.timeSeconds:F2}s";
            Vector2 tooltipSize = GUI.skin.box.CalcSize(new GUIContent(tooltip));
            Rect tooltipRect = new Rect(mousePos.x + 10f, mousePos.y - 10f, tooltipSize.x + 10f, tooltipSize.y + 5f);

            if (tooltipRect.xMax > state.WindowRect.width)
            {
                tooltipRect.x = mousePos.x - tooltipRect.width - 10f;
            }

            if (tooltipRect.yMax > state.WindowRect.height)
            {
                tooltipRect.y = mousePos.y - tooltipRect.height - 10f;
            }

            GUI.Box(tooltipRect, tooltip);
        }
    }
}
