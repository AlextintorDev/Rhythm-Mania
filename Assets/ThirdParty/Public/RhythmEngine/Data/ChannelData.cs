using System.Collections.Generic;
using UnityEngine;

namespace RhythmEngine.Data
{
    public enum ChannelType
    {
        Notes,
        Waveform
    }

    [System.Serializable]
    public class ChannelData
    {
        public string name = "New Channel";
        public ChannelType type = ChannelType.Notes;
        public float height = 65f;
        public bool isExpanded = true;
        
        // For Note channels
        public TrackData trackData;
        
        public ChannelData(string name, ChannelType type)
        {
            this.name = name;
            this.type = type;
            this.height = type == ChannelType.Waveform ? 75f : 65f;
            
            if (type == ChannelType.Notes)
            {
                trackData = new TrackData(name);
            }
        }
    }
}
