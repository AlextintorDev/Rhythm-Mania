using RhythmEngine.Data;
using UnityEditor;
using UnityEngine;

namespace RhythmEngine.Editor
{
    internal sealed class GridInputHandler
    {
        private readonly EditorState state;
        private readonly NoteRenderer noteRenderer;
        private readonly PlaybackController playbackController;

        public GridInputHandler(EditorState state, NoteRenderer noteRenderer, PlaybackController playbackController)
        {
            this.state = state;
            this.noteRenderer = noteRenderer;
            this.playbackController = playbackController;
        }

        public void HandleKeyboardShortcuts()
        {
            Event currentEvent = Event.current;
            if (currentEvent == null ||
                currentEvent.type != EventType.KeyDown ||
                state.CurrentSong?.clip == null ||
                EditorGUIUtility.editingTextField)
            {
                return;
            }

            if (currentEvent.keyCode == KeyCode.Space)
            {
                if (state.IsPlaying)
                {
                    playbackController.Pause();
                }
                else
                {
                    playbackController.Start();
                }

                currentEvent.Use();
                return;
            }

            if (currentEvent.keyCode == KeyCode.Home || currentEvent.keyCode == KeyCode.Escape)
            {
                playbackController.Stop();
                currentEvent.Use();
            }
        }

        public void HandleInput(Rect contentRect)
        {
            if (state.CurrentSong == null)
            {
                return;
            }

            Event currentEvent = Event.current;
            Vector2 mousePos = currentEvent.mousePosition;

            bool insideGrid = mousePos.x > contentRect.x + state.ChannelHeaderWidth &&
                              mousePos.y > contentRect.y + state.TimelineHeight;
            bool insideRuler = mousePos.x > contentRect.x + state.ChannelHeaderWidth &&
                               mousePos.y >= contentRect.y &&
                               mousePos.y <= contentRect.y + state.TimelineHeight;

            if (!insideGrid && !insideRuler &&
                currentEvent.type != EventType.MouseUp &&
                currentEvent.type != EventType.MouseDrag)
            {
                return;
            }

            float localY = mousePos.y - (contentRect.y + state.TimelineHeight);
            int channelIndex = FindChannelIndex(localY);
            float localX = mousePos.x - (contentRect.x + state.ChannelHeaderWidth);
            float time = localX / state.PixelsPerSecond;

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && insideRuler)
            {
                playbackController.Seek(state.CurrentSong.SecondsToBeats(Mathf.Max(0f, time)));
                currentEvent.Use();
                return;
            }

            float beat = state.CurrentSong.SecondsToBeats(time);
            float quantizedBeat = QuantizeBeat(beat);

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && insideGrid)
            {
                HandleLeftMouseDown(channelIndex, mousePos, contentRect, quantizedBeat, currentEvent);
                return;
            }

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1 && insideGrid)
            {
                HandleRightMouseDown(channelIndex, mousePos, contentRect, currentEvent);
                return;
            }

            if (currentEvent.type == EventType.MouseDrag && state.IsDraggingNote && state.DraggingNote != null)
            {
                HandleMouseDrag(quantizedBeat, currentEvent);
                return;
            }

            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0 && state.IsDraggingNote)
            {
                state.IsDraggingNote = false;
                state.DraggingNote = null;
                state.DraggingChannelIndex = -1;
                currentEvent.Use();
            }
        }

        private int FindChannelIndex(float localY)
        {
            float currentY = 0f;
            for (int i = 0; i < state.CurrentSong.channels.Count; i++)
            {
                if (localY >= currentY && localY < currentY + state.CurrentSong.channels[i].height)
                {
                    return i;
                }

                currentY += state.CurrentSong.channels[i].height;
            }

            return -1;
        }

        private float QuantizeBeat(float beat)
        {
            if (!state.SnapToGrid)
            {
                return beat;
            }

            float step = 1f / state.GridDivision;
            return Mathf.Round(beat / step) * step;
        }

        private void HandleLeftMouseDown(int channelIndex, Vector2 mousePos, Rect contentRect, float quantizedBeat, Event currentEvent)
        {
            if (channelIndex == -1)
            {
                return;
            }

            ChannelData channel = state.CurrentSong.channels[channelIndex];
            if (channel.type != ChannelType.Notes || channel.trackData == null)
            {
                return;
            }

            NoteData hitNote = noteRenderer.FindNoteAtPosition(channel, mousePos, contentRect, channelIndex);
            if (hitNote != null)
            {
                state.IsDraggingNote = true;
                state.DraggingNote = hitNote;
                state.DraggingChannelIndex = channelIndex;
                state.DragStartBeat = hitNote.beat;
                state.DragStartMouse = mousePos;
                currentEvent.Use();
                return;
            }

            Undo.RecordObject(state.CurrentSong, "Add Note");
            state.CurrentSong.ToggleNoteAt(channelIndex, quantizedBeat);
            state.MarkSongDirty();
            currentEvent.Use();
        }

        private void HandleRightMouseDown(int channelIndex, Vector2 mousePos, Rect contentRect, Event currentEvent)
        {
            if (channelIndex == -1)
            {
                return;
            }

            ChannelData channel = state.CurrentSong.channels[channelIndex];
            if (channel.type != ChannelType.Notes || channel.trackData == null)
            {
                return;
            }

            NoteData hitNote = noteRenderer.FindNoteAtPosition(channel, mousePos, contentRect, channelIndex);
            if (hitNote == null)
            {
                return;
            }

            Undo.RecordObject(state.CurrentSong, "Delete Note");
            channel.trackData.RemoveNoteAt(hitNote.beat, 0.001f);
            state.MarkSongDirty();
            currentEvent.Use();
        }

        private void HandleMouseDrag(float quantizedBeat, Event currentEvent)
        {
            if (state.DraggingChannelIndex < 0 || state.DraggingChannelIndex >= state.CurrentSong.channels.Count)
            {
                return;
            }

            ChannelData channel = state.CurrentSong.channels[state.DraggingChannelIndex];
            if (channel.type != ChannelType.Notes || channel.trackData == null)
            {
                return;
            }

            float newBeat = Mathf.Max(0f, quantizedBeat);
            if (!Mathf.Approximately(state.DraggingNote.beat, newBeat))
            {
                Undo.RecordObject(state.CurrentSong, "Move Note");
                state.DraggingNote.beat = newBeat;
                state.DraggingNote.timeSeconds = state.CurrentSong.BeatsToSeconds(newBeat);
                state.MarkSongDirty();
            }

            state.RequestRepaint();
            currentEvent.Use();
        }
    }
}
