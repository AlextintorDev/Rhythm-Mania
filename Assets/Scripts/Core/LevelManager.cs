using System.Collections;
using System.Threading.Tasks;
using GameSaveSystem.Core;
using MoreMountains.Tools;
using RhythmEngine.Data;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [Header("Game")]
    const int MAX_HP = 100;
    [SerializeField] private float hp = MAX_HP;
    [SerializeField] private float hpIncreaseOnHit = 5f;
    [SerializeField] private float hpDecreaseOnMiss = 10f;
    public float HpPercentage => hp / MAX_HP;

    [Header("Level")]
    [SerializeField] private SongItem songItem;
    [SerializeField] private BeatScroller beatScroller;
    [SerializeField] private BackgroundPlayerController backgroundPlayerController;
    [SerializeField] private GameUI gameUI;

    [Header("Feedback")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip missSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip winSound;

    void Awake()
    {
        GameEventBus.Subscribe<HitEvent>(OnHitEvent);
        AnalyticsRecorder.StartNewTry(GameSettings.Instance.SongIndex);
    }

    void OnDisable()
    {
        GameEventBus.Unsubscribe<HitEvent>(OnHitEvent);
    }

    private bool checkGameOver = true;

    void Update()
    {
        if(checkGameOver && beatScroller.IsSongFinished())
        {
            checkGameOver = false;
            PlaySound(winSound);
            GameEventBus.Invoke(new GameOverEvent(true));
            StartCoroutine(GameOverRoutine());
        }
    }

    IEnumerator GameOverRoutine()
    {
        ValuesManager.SetString("bcode", AnalyticsRecorder.GetShareableCode());
        yield return new WaitForSecondsRealtime(3f);
        Metadata.SetMetadata("fromGameScene", true.ToString());
        GameManager.Instance.GoToScene("MainMenu");
    }

    private void OnHitEvent(HitEvent hitEvent)
    {
        bool isHit = hitEvent.Success;

        if(isHit)
        {
            hp = Mathf.Min(hp + hpIncreaseOnHit, MAX_HP);
            //Feedback
            PlaySound(hitSound);
        }
        else
        {
            hp = Mathf.Max(hp - hpDecreaseOnMiss, 0);
            //Feedback
            PlaySound(missSound, 0.12f);
        }

        if(hp <= 0 && checkGameOver)
        {
            checkGameOver = false;
            PlaySound(gameOverSound);
            GameEventBus.Invoke(new GameOverEvent(false));
            StartCoroutine(GameOverRoutine());
        }

        gameUI.UpdateEnergyBar(HpPercentage);
    }

    public void StartLevel()
    {
        beatScroller.Initialize(GameSettings.Instance.GetSong());
        beatScroller.StartSong();
        backgroundPlayerController.PlayBackgroundVideo();
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if(clip == null)
            return;

        MMSoundManagerSoundPlayEvent.Trigger(
                clip,
                MMSoundManager.MMSoundManagerTracks.Sfx,
                this.transform.position,
                volume: volume
            );
    }

    public SongItem GetCurrentSong() => GameSettings.Instance.GetSong();
}

public class HitEvent : IGameEvent
{
    public HitType hitType;
    public Note note;
    public bool Success => hitType != HitType.Miss;
    public HitEvent(HitType hitType, Note note)
    {
        this.hitType = hitType;
        this.note = note;
    }
}

public class GameOverEvent : IGameEvent
{
    public bool win;
    public GameOverEvent(bool win)
    {
        this.win = win;
    }
}

public enum HitType
{
    Perfect,
    Good,
    Miss
}