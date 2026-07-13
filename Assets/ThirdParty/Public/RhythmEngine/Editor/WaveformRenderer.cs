using System.Collections.Generic;
using UnityEngine;

namespace RhythmEngine.Editor
{
    internal sealed class WaveformRenderer
    {
        private readonly EditorState state;
        private readonly Dictionary<string, Texture2D> waveformCache = new Dictionary<string, Texture2D>();

        public WaveformRenderer(EditorState state)
        {
            this.state = state;
        }

        public void Dispose()
        {
            foreach (Texture2D texture in waveformCache.Values)
            {
                if (texture != null)
                {
                    Object.DestroyImmediate(texture);
                }
            }

            waveformCache.Clear();
        }

        public void DrawWaveform(Rect rect)
        {
            if (state.CurrentSong?.clip == null)
            {
                return;
            }

            int waveformWidth = Mathf.Clamp(
                Mathf.CeilToInt(state.CurrentSong.clip.length * state.PixelsPerSecond),
                512,
                8192);
            string cacheKey = GetWaveformCacheKey(state.CurrentSong.clip, waveformWidth);

            if (!waveformCache.TryGetValue(cacheKey, out Texture2D texture))
            {
                CleanupWaveformCache(state.CurrentSong.clip, cacheKey);
                texture = WaveformUtils.GetWaveformTexture(
                    state.CurrentSong.clip,
                    waveformWidth,
                    Mathf.Max(32, Mathf.RoundToInt(rect.height)),
                    new Color(1f, 0.5f, 0f, 0.8f));

                if (texture != null)
                {
                    waveformCache[cacheKey] = texture;
                }
            }

            if (texture == null)
            {
                return;
            }

            float width = state.CurrentSong.clip.length * state.PixelsPerSecond;
            Rect waveRect = new Rect(rect.x, rect.y, width, rect.height);
            GUI.DrawTexture(waveRect, texture, ScaleMode.StretchToFill, true);
        }

        private static string GetWaveformCacheKey(AudioClip clip, int width)
        {
            return $"{clip.GetInstanceID()}_{width}";
        }

        private void CleanupWaveformCache(AudioClip clip, string keepKey)
        {
            string keyPrefix = clip.GetInstanceID() + "_";
            List<string> keysToRemove = null;

            foreach (KeyValuePair<string, Texture2D> entry in waveformCache)
            {
                if (entry.Key == keepKey || !entry.Key.StartsWith(keyPrefix))
                {
                    continue;
                }

                if (keysToRemove == null)
                {
                    keysToRemove = new List<string>();
                }

                keysToRemove.Add(entry.Key);
            }

            if (keysToRemove == null)
            {
                return;
            }

            foreach (string key in keysToRemove)
            {
                Texture2D texture = waveformCache[key];
                if (texture != null)
                {
                    Object.DestroyImmediate(texture);
                }

                waveformCache.Remove(key);
            }
        }
    }
}
