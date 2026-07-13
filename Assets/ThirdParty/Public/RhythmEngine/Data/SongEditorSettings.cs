using UnityEngine;

namespace RhythmEngine.Data
{
    [CreateAssetMenu(fileName = "SongEditorSettings", menuName = "Rhythm/Song Editor Settings")]
    public class SongEditorSettings : ScriptableObject
    {
        [Header("Playback Settings")]
        [Tooltip("Play a sound when the playhead passes over a note during preview")]
        public bool playSoundInSongEditor = false;
        
        [Tooltip("Audio clip to play when a note is triggered during preview")]
        public AudioClip notePreviewSound;
        
        [Header("Editor Settings")]
        [Range(0f, 1f)]
        [Tooltip("Volume for note preview sounds")]
        public float previewVolume = 0.5f;

        [Tooltip("Maximum allowed grid division for visual grid and snapping (1/x)")]
        [Min(1)]
        public int maxGridDivision = 32;

        [Header("Visuals")]
        [Tooltip("Icon/texture used to draw beats in the grid")]
        public Texture2D beatIcon;
        
        [Tooltip("Tint color for the beat icon")]
        public Color beatIconTint = Color.green;

        [Tooltip("Shade alternate track columns in the editor grid")] 
        public bool shadeAlternateColumns = true;

        [Tooltip("Show tooltips when hovering notes in the editor grid")] 
        public bool showTooltips = true;
    }
}