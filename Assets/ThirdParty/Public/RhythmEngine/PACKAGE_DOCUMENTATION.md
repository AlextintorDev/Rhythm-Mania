# Rhythm Engine Package - Complete Implementation

## Package Overview

This package provides a complete rhythm game song structure and visual editor for Unity. It includes:

### Core Components

1. **SongItem.cs** - Main ScriptableObject for song data
2. **TrackData.cs** - Track container with note management
3. **NoteData.cs** - Individual note with extensible payload system
4. **SongEditorWindow.cs** - Visual timeline editor
5. **SimpleMidiImporter.cs** - Basic MIDI import functionality
6. **EditorHelpers.cs** - Utility functions for time conversion and UI
7. **NoteDataDrawers.cs** - Custom property drawers for inspector

### Features Implemented

? **ScriptableObject Song Structure**
- AudioClip reference for preview
- BPM and firstBeatOffset for timing
- Dynamic track management
- Serializable note system with payload

? **Visual Timeline Editor**
- Left panel with configuration controls
- Central grid with timeline and track columns
- Click-to-toggle note editing
- Audio preview with playhead
- Scroll support for large songs

? **Grid Configuration**
- Beat step quantization (1, 1/2, 1/4, 1/8)
- Configurable track count
- Visual beat labels and track headers
- Color-coded notes (green active, gray inactive)

? **MIDI Import**
- Basic MIDI file parsing (simplified)
- Automatic track creation
- Note quantization to grid
- Menu integration for asset workflow

? **Extensible Note System**
- NotePayload with metadata, id, velocity
- Custom property drawers for editing
- Future-proof for additional game data

? **Time Conversion Utilities**
- Beats ? seconds conversion
- Quantization functions
- Playback synchronization

### Usage Instructions

#### 1. Creating Songs
```
Right-click in Project ? Create ? Rhythm ? Song Item
```

#### 2. Opening Editor
```
Window ? Rhythm Sequencer
```

#### 3. Basic Workflow
1. Create or select SongItem
2. Configure tracks and beat step
3. Click grid cells to add/remove notes
4. Use preview controls to test timing
5. Save when complete

#### 4. MIDI Import
```
Right-click .mid file ? Import MIDI to Song (Simple)
```

### Technical Specifications

#### Beat System
- 0-based beat indexing
- Float precision for fractional beats
- Quantization to grid steps
- Support for sustained notes via length property

#### Time Conversion
```csharp
float secondsPerBeat = 60f / bpm;
float timeInSeconds = firstBeatOffset + beat * secondsPerBeat;
float quantizedBeat = Mathf.Round(beat / step) * step;
```

#### Grid Representation
- Columns = Tracks (horizontal)
- Rows = Beat steps (vertical)
- Cell size = 24px default
- Scroll support for large timelines

#### Visual Elements
- Active notes: Green background + white dot
- Inactive cells: Gray background
- Grid lines: Semi-transparent gray
- Playhead: Red line during preview
- Timeline labels: Beat indices on left

### API Reference

#### SongItem Methods
```csharp
// Track management
void SetTrackCount(int count)
void AddTrack(string tag)
void RemoveTrack(int index)

// Note editing
void ToggleNoteAt(int trackIndex, float beat)
NoteData GetNoteAt(int trackIndex, float beat)
bool HasNoteAt(int trackIndex, float beat)
void ClearAllNotes()

// Time utilities
float BeatsToSeconds(float beats)
float SecondsToBeats(float seconds)
float QuantizeBeat(float beat, float step)
```

#### TrackData Methods
```csharp
// Note management
void AddNote(NoteData note)
bool RemoveNoteAt(float beat, float tolerance = 0.01f)
NoteData GetNoteAt(float beat, float tolerance = 0.01f)
void ClearNotes()
```

### Extension Points

#### Custom Note Types
Extend NotePayload for game-specific data:
```csharp
[System.Serializable]
public class NotePayload
{
    public string metadata;
    public int id;
    public float velocity;
    // Add custom fields here
    public NoteType noteType;
    public Color visualColor;
    public AudioClip customSound;
}
```

#### Advanced MIDI Import
For production use, integrate DryWetMIDI:
```csharp
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
// Implement full MIDI parsing with tempo maps
```

#### Runtime Integration
Access song data in gameplay:
```csharp
public class RhythmGameController : MonoBehaviour
{
    public SongItem currentSong;
    
    void Update()
    {
        float currentBeat = currentSong.SecondsToBeats(audioSource.time);
        
        for (int track = 0; track < currentSong.tracks.Count; track++)
        {
            if (currentSong.HasNoteAt(track, currentBeat))
            {
                // Handle note hit
            }
        }
    }
}
```

### File Structure
```
Assets/RhythmEngine/
??? Data/
?   ??? SongItem.cs
?   ??? TrackData.cs
?   ??? NoteData.cs
??? Editor/
?   ??? SongEditorWindow.cs
?   ??? SimpleMidiImporter.cs
?   ??? EditorHelpers.cs
?   ??? NoteDataDrawers.cs
??? Examples/
?   ??? ExampleSong.asset
??? README.md
```

### Known Limitations

1. **MIDI Import**: Basic parser - complex tempo maps not supported
2. **Performance**: Recommended max 128 visible beats, 16 tracks
3. **Audio**: Preview only - no real-time recording
4. **Visual**: Fixed cell size, basic grid layout

### Future Enhancements

1. **Advanced MIDI**: Full DryWetMIDI integration
2. **Visual**: Customizable note colors, shapes
3. **Audio**: Real-time recording, metronome
4. **Export**: JSON/XML export for external tools
5. **Validation**: Note collision detection, timing validation

### Compatibility

- Unity 2019.4+ / 2020.3+ LTS
- .NET Framework 4.7.1
- Windows/Mac/Linux editor support
- No runtime dependencies

This package provides a solid foundation for rhythm game development with room for customization and extension based on specific game requirements.