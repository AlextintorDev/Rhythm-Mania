using System.Collections.Generic;
using UnityEngine;

namespace RhythmEngine.Data
{
    [System.Serializable]
    public class TrackData
    {
        public string tag = "Track";
        public List<NoteData> notes = new List<NoteData>();
        
        public TrackData()
        {
        }
        
        public TrackData(string tag)
        {
            this.tag = tag;
            this.notes = new List<NoteData>();
        }
        
        public NoteData GetNoteAt(float beat, float tolerance = 0.01f)
        {
            for (int i = 0; i < notes.Count; i++)
            {
                if (Mathf.Abs(notes[i].beat - beat) <= tolerance)
                {
                    return notes[i];
                }
            }
            return null;
        }
        
        public void AddNote(NoteData note)
        {
            // Remove existing note at same beat first
            RemoveNoteAt(note.beat);
            
            // Insert note in sorted order
            int insertIndex = 0;
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].beat > note.beat)
                {
                    insertIndex = i;
                    break;
                }
                insertIndex = i + 1;
            }
            
            notes.Insert(insertIndex, note);
        }
        
        public bool RemoveNoteAt(float beat, float tolerance = 0.01f)
        {
            for (int i = 0; i < notes.Count; i++)
            {
                if (Mathf.Abs(notes[i].beat - beat) <= tolerance)
                {
                    notes.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        
        public void ClearNotes()
        {
            notes.Clear();
        }
    }
}