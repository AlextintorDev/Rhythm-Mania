using UnityEngine;

namespace RhythmEngine.Editor
{
    public static class EditorHelpers
    {
        // Time conversion utilities
        public static float BeatsToSeconds(float beats, float bpm, float firstBeatOffset)
        {
            float secondsPerBeat = 60f / bpm;
            return firstBeatOffset + beats * secondsPerBeat;
        }
        
        public static float SecondsToBeats(float seconds, float bpm, float firstBeatOffset)
        {
            float secondsPerBeat = 60f / bpm;
            return (seconds - firstBeatOffset) / secondsPerBeat;
        }
        
        // Quantization
        public static float QuantizeBeat(float beat, float step)
        {
            return Mathf.Round(beat / step) * step;
        }
        
        // Grid helpers
        public static Rect GetCellRect(int row, int column, float cellSize, Vector2 gridOffset)
        {
            return new Rect(
                gridOffset.x + column * cellSize,
                gridOffset.y + row * cellSize,
                cellSize,
                cellSize
            );
        }
        
        // Colors for UI
        public static readonly Color ActiveNoteColor = new Color(0.2f, 0.8f, 0.2f, 1f); // Green
        public static readonly Color InactiveNoteColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Gray
        public static readonly Color GridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        public static readonly Color PlayheadColor = new Color(1f, 0.2f, 0.2f, 0.8f); // Red
        
        // Beat step options
        public static readonly float[] BeatStepOptions = { 1f, 0.5f, 0.25f, 0.125f, 0.0625f, 0.03125f };
        public static readonly string[] BeatStepLabels = { "1", "1/2", "1/4", "1/8", "1/16", "1/32" };
        
        // Beat length presets
        public static readonly float[] BeatLengthPresets = { 0.25f, 0.5f, 1f, 1.5f, 2f, 4f };
        public static readonly string[] BeatLengthLabels = { "1/4", "1/2", "1", "1.5", "2", "4" };
        
        public static string FormatBeatTime(float beat)
        {
            int wholeBeat = Mathf.FloorToInt(beat);
            float fraction = beat - wholeBeat;
            
            if (Mathf.Approximately(fraction, 0f))
            {
                return wholeBeat.ToString();
            }
            else if (Mathf.Approximately(fraction, 0.25f))
            {
                return $"{wholeBeat}.25";
            }
            else if (Mathf.Approximately(fraction, 0.5f))
            {
                return $"{wholeBeat}.5";
            }
            else if (Mathf.Approximately(fraction, 0.75f))
            {
                return $"{wholeBeat}.75";
            }
            else
            {
                return beat.ToString("F2");
            }
        }
    }
}