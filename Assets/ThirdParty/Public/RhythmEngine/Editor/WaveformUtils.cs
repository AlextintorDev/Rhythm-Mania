using UnityEngine;

namespace RhythmEngine.Editor
{
    public static class WaveformUtils
    {
        public static Texture2D GetWaveformTexture(AudioClip clip, int width, int height, Color color)
        {
            if (clip == null) return null;

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] colors = new Color[width * height];
            
            // Clear background (transparent)
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(0, 0, 0, 0);
            }

            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            int packSize = (samples.Length / width) + 1;
            
            for (int x = 0; x < width; x++)
            {
                float max = 0;
                for (int i = 0; i < packSize; i++)
                {
                    int index = x * packSize + i;
                    if (index < samples.Length)
                    {
                        float val = Mathf.Abs(samples[index]);
                        if (val > max) max = val;
                    }
                }

                int h = (int)(max * height);
                int yStart = (height - h) / 2;
                
                for (int y = 0; y < h; y++)
                {
                    colors[(yStart + y) * width + x] = color;
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }
    }
}
