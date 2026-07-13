namespace RhythmEngine.Data
{
    [System.Serializable]
    public class NotePayload
    {
        public NotePayload()
        {
        }
    }

    [System.Serializable]
    public class NoteData
    {
        // Beat position (kept for backward compatibility with existing features like playback)
        public float beat;
        
        // Exact time position in seconds (authoritative for editing/moving)
        public float timeSeconds;
        
        public float length = 1.0f; // Beat length for sustained notes
        public NotePayload payload = new NotePayload();
        
        public NoteData()
        {
        }
        
        public NoteData(float beat, float length = 1.0f)
        {
            this.beat = beat;
            this.timeSeconds = 0f; // Caller/editor should set an exact time using song BPM and offset
            this.length = length;
            this.payload = new NotePayload();
        }
    }
}