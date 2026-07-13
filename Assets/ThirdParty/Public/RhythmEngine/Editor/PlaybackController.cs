using RhythmEngine.Data;
using UnityEngine;

namespace RhythmEngine.Editor
{
    internal sealed class PlaybackController
    {
        private readonly EditorState state;

        public PlaybackController(EditorState state)
        {
            this.state = state;
        }

        public void Initialize()
        {
            GameObject audioGO = new GameObject("PreviewAudioSource");
            audioGO.hideFlags = HideFlags.HideAndDontSave;
            state.PreviewAudioSource = audioGO.AddComponent<AudioSource>();
            state.PreviewAudioSource.playOnAwake = false;

            GameObject noteGO = new GameObject("PreviewNoteAudioSource");
            noteGO.hideFlags = HideFlags.HideAndDontSave;
            state.PreviewNoteSound = noteGO.AddComponent<AudioSource>();
            state.PreviewNoteSound.playOnAwake = false;

            SyncCurrentSongClip();
        }

        public void Dispose()
        {
            Stop();

            if (state.PreviewAudioSource != null)
            {
                Object.DestroyImmediate(state.PreviewAudioSource.gameObject);
                state.PreviewAudioSource = null;
            }

            if (state.PreviewNoteSound != null)
            {
                Object.DestroyImmediate(state.PreviewNoteSound.gameObject);
                state.PreviewNoteSound = null;
            }
        }

        public void SyncCurrentSongClip()
        {
            if (state.PreviewAudioSource != null)
            {
                state.PreviewAudioSource.clip = state.CurrentSong != null ? state.CurrentSong.clip : null;
            }
        }

        public void Start()
        {
            if (state.CurrentSong?.clip == null || state.PreviewAudioSource == null)
            {
                return;
            }

            if (state.PreviewAudioSource.clip != state.CurrentSong.clip)
            {
                state.PreviewAudioSource.clip = state.CurrentSong.clip;
            }

            if (state.IsPaused)
            {
                state.IsPaused = false;
                state.IsPlaying = true;
                state.PreviousPlaybackBeat = state.CurrentPlaybackBeat;
                state.PreviewAudioSource.UnPause();
                return;
            }

            float startTime = state.CurrentPlaybackBeat > 0f
                ? state.CurrentSong.BeatsToSeconds(state.CurrentPlaybackBeat)
                : state.CurrentSong.firstBeatOffset;
            startTime = Mathf.Clamp(startTime, 0f, Mathf.Max(0f, state.CurrentSong.clip.length - 0.001f));

            state.PreviewAudioSource.time = startTime;
            state.CurrentPlaybackBeat = state.CurrentSong.SecondsToBeats(startTime);
            state.PreviousPlaybackBeat = state.CurrentPlaybackBeat;
            state.TriggeredNotesThisFrame.Clear();
            state.IsPaused = false;
            state.IsPlaying = true;
            state.PreviewAudioSource.Play();
        }

        public void Pause()
        {
            state.IsPlaying = false;
            state.IsPaused = true;
            if (state.PreviewAudioSource != null)
            {
                state.PreviewAudioSource.Pause();
            }
        }

        public void Stop()
        {
            state.IsPlaying = false;
            state.IsPaused = false;
            state.CurrentPlaybackBeat = 0f;
            state.PreviousPlaybackBeat = 0f;
            state.TriggeredNotesThisFrame.Clear();
            if (state.PreviewAudioSource != null)
            {
                state.PreviewAudioSource.Stop();
            }
        }

        public void Seek(float beat)
        {
            if (state.CurrentSong == null)
            {
                return;
            }

            float clampedBeat = Mathf.Clamp(beat, 0f, state.GetSongLengthBeats());
            float targetTime = Mathf.Clamp(
                state.CurrentSong.BeatsToSeconds(clampedBeat),
                0f,
                Mathf.Max(0f, state.GetSongLengthSeconds() - 0.001f));

            state.CurrentPlaybackBeat = state.CurrentSong.SecondsToBeats(targetTime);
            state.PreviousPlaybackBeat = state.CurrentPlaybackBeat;
            state.TriggeredNotesThisFrame.Clear();

            if (state.PreviewAudioSource != null)
            {
                state.PreviewAudioSource.time = targetTime;
            }

            state.RequestRepaint();
        }

        public void Update()
        {
            if (state.CurrentSong == null || state.PreviewAudioSource == null || !state.IsPlaying)
            {
                return;
            }

            if (!state.PreviewAudioSource.isPlaying)
            {
                state.IsPlaying = false;
                state.IsPaused = false;
                state.CurrentPlaybackBeat = state.GetSongLengthBeats();
                state.PreviousPlaybackBeat = state.CurrentPlaybackBeat;
                state.TriggeredNotesThisFrame.Clear();
                return;
            }

            state.PreviousPlaybackBeat = state.CurrentPlaybackBeat;
            state.CurrentPlaybackBeat = state.CurrentSong.SecondsToBeats(state.PreviewAudioSource.time);

            if (state.EditorSettings != null &&
                state.EditorSettings.playSoundInSongEditor &&
                state.EditorSettings.notePreviewSound != null &&
                state.PreviewNoteSound != null)
            {
                TriggerNotePreviewSounds();
            }
        }

        private void TriggerNotePreviewSounds()
        {
            foreach (ChannelData channel in state.CurrentSong.channels)
            {
                if (channel.type != ChannelType.Notes || channel.trackData == null)
                {
                    continue;
                }

                foreach (NoteData note in channel.trackData.notes)
                {
                    if (note.beat <= state.PreviousPlaybackBeat || note.beat > state.CurrentPlaybackBeat)
                    {
                        continue;
                    }

                    string noteKey = $"{channel.name}_{note.beat}";
                    if (state.TriggeredNotesThisFrame.Contains(noteKey))
                    {
                        continue;
                    }

                    state.TriggeredNotesThisFrame.Add(noteKey);
                    state.PreviewNoteSound.clip = state.EditorSettings.notePreviewSound;
                    state.PreviewNoteSound.volume = state.EditorSettings.previewVolume;
                    state.PreviewNoteSound.PlayOneShot(
                        state.EditorSettings.notePreviewSound,
                        state.EditorSettings.previewVolume);
                }
            }

            if (state.CurrentPlaybackBeat < state.PreviousPlaybackBeat)
            {
                state.TriggeredNotesThisFrame.Clear();
            }
        }
    }
}
