using System.Collections;
using RhythmEngine.Data;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    [Header("References")] 
    //0 - blue, 1 - yellow, 2 - red, 3 - green
    [SerializeField] private GameObject[] rails;
    [SerializeField] private Note notePrefab;

    [Header("Level")] 
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float beatTempo;

    [Header("Debug")]
    [SerializeField] private bool hasStarted;
    [SerializeField] private double startTime;

    void Awake()
    {
        GameEventBus.Subscribe<GameOverEvent>(OnGameOver);
    }

    void OnDisable()
    {
        GameEventBus.Unsubscribe<GameOverEvent>(OnGameOver);
    }

    void OnGameOver(GameOverEvent _)
    {
        musicSource.Stop();
        enabled = false;
    }

    public void Initialize(SongItem song)
    {
        GameSettings settings = GameSettings.Instance;

        musicSource.clip = song.clip;
        beatTempo = song.bpm;

        //Asegurarse de spawnear las notas de solo los 4 primeros canales de notas
        int channelsSpawned = 0;
        song.channels.ForEach(channel =>
        {
            if(channel.type == ChannelType.Notes && channelsSpawned < 4)
            {
                channel.trackData.notes.ForEach(note =>
                {
                    Note noteObj = Instantiate(notePrefab, rails[channelsSpawned].transform);
                    noteObj.Initialize((note.beat * (60f / beatTempo)) + settings.FirstBeatOffset + AudioSettings.dspTime, channelsSpawned, settings.NoteSpeed); //Ajustar el tiempo de spawn para que aparezcan en el momento correcto, se le suma el offset para que aparezcan antes de que empiece la musica

                });
                channelsSpawned++;
            }
            
        });

        hasStarted = false;
        startTime = 0;
    }

    public void StartSong()
    {
        GameSettings settings = GameSettings.Instance;

        hasStarted = true;
        startTime = AudioSettings.dspTime;
        musicSource.PlayScheduled(startTime + settings.FirstBeatOffset);    
    }

    public bool IsSongFinished()
    {
        return !musicSource.isPlaying && hasStarted;
    }
    
    public float GetCurrentBeat()
    {
        if (!hasStarted) return 0f;

        double elapsedTime = AudioSettings.dspTime - startTime;
        float currentBeat = (float)((elapsedTime - GameSettings.Instance.FirstBeatOffset) / (60f / beatTempo));
        return currentBeat;
    }
}
