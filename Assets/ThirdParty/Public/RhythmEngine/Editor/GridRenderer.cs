using RhythmEngine.Data;
using System;
using UnityEditor;
using UnityEngine;

namespace RhythmEngine.Editor
{
    internal sealed class GridRenderer
    {
        private readonly EditorState state;
        private readonly NoteRenderer noteRenderer;
        private readonly WaveformRenderer waveformRenderer;

        public GridRenderer(EditorState state, NoteRenderer noteRenderer, WaveformRenderer waveformRenderer)
        {
            this.state = state;
            this.noteRenderer = noteRenderer;
            this.waveformRenderer = waveformRenderer;
        }

        public void DrawMainGrid(Action<Rect> inputHandler)
        {
            if (state.CurrentSong == null)
            {
                return;
            }

            float songLengthSec = state.GetSongLengthSeconds();
            float contentWidth = Mathf.Max(
                state.WindowRect.width - state.LeftPanelWidth,
                songLengthSec * state.PixelsPerSecond + 100f);

            float totalHeight = state.TimelineHeight;
            foreach (ChannelData channel in state.CurrentSong.channels)
            {
                totalHeight += channel.height;
            }

            state.GridScrollPos = EditorGUILayout.BeginScrollView(state.GridScrollPos);
            Rect contentRect = GUILayoutUtility.GetRect(contentWidth, totalHeight);
            EditorGUI.DrawRect(contentRect, state.GridBackgroundColor);

            Rect timelineRect = new Rect(
                contentRect.x + state.ChannelHeaderWidth,
                contentRect.y,
                contentRect.width - state.ChannelHeaderWidth,
                state.TimelineHeight);
            DrawTimeRuler(timelineRect);

            float currentY = contentRect.y + state.TimelineHeight;
            for (int i = 0; i < state.CurrentSong.channels.Count; i++)
            {
                ChannelData channel = state.CurrentSong.channels[i];
                Rect channelRect = new Rect(contentRect.x, currentY, contentRect.width, channel.height);
                Rect headerRect = new Rect(channelRect.x, channelRect.y, state.ChannelHeaderWidth, channelRect.height);
                DrawChannelHeader(headerRect, channel, i);

                Rect bodyRect = new Rect(
                    channelRect.x + state.ChannelHeaderWidth,
                    channelRect.y,
                    channelRect.width - state.ChannelHeaderWidth,
                    channelRect.height);
                DrawChannelContent(bodyRect, channel, i);

                EditorGUI.DrawRect(
                    new Rect(contentRect.x, currentY + channel.height - 1f, contentRect.width, 1f),
                    state.ChannelSeparator);

                currentY += channel.height;
            }

            if (state.IsPlaying || state.CurrentPlaybackBeat > 0f)
            {
                DrawPlayhead(contentRect);
            }

            inputHandler?.Invoke(contentRect);
            EditorGUILayout.EndScrollView();
        }

        private void DrawTimeRuler(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f));
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SlideArrow);

            float startSec = state.GridScrollPos.x / state.PixelsPerSecond;
            float endSec = (state.GridScrollPos.x + state.WindowRect.width) / state.PixelsPerSecond;
            float beatDuration = 60f / state.CurrentSong.bpm;
            float subdivisionPixelWidth = (beatDuration * state.PixelsPerSecond) / Mathf.Max(1, state.GridDivision);
            int labelStep = Mathf.Max(
                1,
                Mathf.CeilToInt(EditorState.MinBeatLabelSpacing / Mathf.Max(1f, beatDuration * state.PixelsPerSecond)));
            int startBeat = Mathf.FloorToInt((startSec - state.CurrentSong.firstBeatOffset) / beatDuration);
            int endBeat = Mathf.CeilToInt((endSec - state.CurrentSong.firstBeatOffset) / beatDuration);

            for (int beatIndex = startBeat; beatIndex <= endBeat; beatIndex++)
            {
                if (beatIndex < 0)
                {
                    continue;
                }

                float time = state.CurrentSong.BeatsToSeconds(beatIndex);
                float x = rect.x + time * state.PixelsPerSecond;
                EditorGUI.DrawRect(new Rect(x, rect.y + 15f, 1f, rect.height - 15f), Color.white);

                if (beatIndex % labelStep == 0)
                {
                    GUI.Label(new Rect(x + 2f, rect.y, 40f, 15f), beatIndex.ToString(), EditorStyles.miniLabel);
                }

                if (subdivisionPixelWidth < EditorState.MinSubdivisionPixelSpacing)
                {
                    continue;
                }

                float step = 1f / state.GridDivision;
                for (int subdivision = 1; subdivision < state.GridDivision; subdivision++)
                {
                    float subdivisionTime = state.CurrentSong.BeatsToSeconds(beatIndex + subdivision * step);
                    float subdivisionX = rect.x + subdivisionTime * state.PixelsPerSecond;
                    if (subdivisionX > x + 5f)
                    {
                        EditorGUI.DrawRect(
                            new Rect(subdivisionX, rect.y + 22f, 1f, rect.height - 22f),
                            new Color(1f, 1f, 1f, 0.3f));
                    }
                }
            }
        }

        private void DrawChannelHeader(Rect rect, ChannelData channel, int index)
        {
            GUI.Box(rect, string.Empty, EditorStyles.helpBox);

            Rect labelRect = new Rect(rect.x + 5f, rect.y + 5f, rect.width - 30f, 20f);
            channel.name = EditorGUI.TextField(labelRect, channel.name);

            Rect deleteRect = new Rect(rect.x + rect.width - 25f, rect.y + 5f, 20f, 20f);
            if (!GUI.Button(deleteRect, "X"))
            {
                return;
            }

            Undo.RecordObject(state.CurrentSong, "Remove Channel");
            state.CurrentSong.RemoveChannel(index);
            state.MarkSongDirty();
            GUIUtility.ExitGUI();
        }

        private void DrawChannelContent(Rect rect, ChannelData channel, int channelIndex)
        {
            DrawGridLines(rect, channelIndex);

            if (channel.type == ChannelType.Waveform)
            {
                waveformRenderer.DrawWaveform(rect);
                return;
            }

            if (channel.type == ChannelType.Notes)
            {
                noteRenderer.DrawNotes(rect, channel);
            }
        }

        private void DrawGridLines(Rect rect, int channelIndex)
        {
            float startSec = Mathf.Max(0f, (state.GridScrollPos.x - state.ChannelHeaderWidth) / state.PixelsPerSecond);
            float endSec = (state.GridScrollPos.x + state.WindowRect.width) / state.PixelsPerSecond;
            float beatDuration = 60f / state.CurrentSong.bpm;
            float subdivisionPixelWidth = (beatDuration * state.PixelsPerSecond) / Mathf.Max(1, state.GridDivision);
            int startBeat = Mathf.FloorToInt((startSec - state.CurrentSong.firstBeatOffset) / beatDuration);
            int endBeat = Mathf.CeilToInt((endSec - state.CurrentSong.firstBeatOffset) / beatDuration);

            for (int beatIndex = startBeat; beatIndex <= endBeat; beatIndex++)
            {
                if (beatIndex < 0)
                {
                    continue;
                }

                float time = state.CurrentSong.BeatsToSeconds(beatIndex);
                float x = rect.x + time * state.PixelsPerSecond;

                if (state.EditorSettings != null && state.EditorSettings.shadeAlternateColumns && beatIndex % 2 == 0)
                {
                    float nextTime = state.CurrentSong.BeatsToSeconds(beatIndex + 1);
                    float nextX = rect.x + nextTime * state.PixelsPerSecond;
                    float columnWidth = nextX - x;
                    EditorGUI.DrawRect(new Rect(x, rect.y, columnWidth, rect.height), new Color(0f, 0f, 0f, 0.1f));
                }

                EditorGUI.DrawRect(new Rect(x, rect.y, 1f, rect.height), state.MajorLine);

                if (subdivisionPixelWidth < EditorState.MinSubdivisionPixelSpacing)
                {
                    continue;
                }

                float step = 1f / state.GridDivision;
                for (int subdivision = 1; subdivision < state.GridDivision; subdivision++)
                {
                    float subdivisionTime = state.CurrentSong.BeatsToSeconds(beatIndex + subdivision * step);
                    float subdivisionX = rect.x + subdivisionTime * state.PixelsPerSecond;
                    EditorGUI.DrawRect(new Rect(subdivisionX, rect.y, 1f, rect.height), state.MinorLine);
                }
            }
        }

        private void DrawPlayhead(Rect contentRect)
        {
            float time = state.CurrentSong.BeatsToSeconds(state.CurrentPlaybackBeat);
            float x = contentRect.x + state.ChannelHeaderWidth + time * state.PixelsPerSecond;
            EditorGUI.DrawRect(new Rect(x, contentRect.y, 2f, contentRect.height), Color.red);

            if (!state.FollowPlayhead || !state.IsPlaying)
            {
                return;
            }

            float targetX = x - state.ChannelHeaderWidth - (state.WindowRect.width - state.ChannelHeaderWidth) / 2f;
            state.GridScrollPos.x = Mathf.Lerp(state.GridScrollPos.x, targetX, Time.deltaTime * 5f);
        }
    }
}
