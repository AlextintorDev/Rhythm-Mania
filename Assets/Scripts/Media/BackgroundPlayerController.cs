using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class BackgroundPlayerController : MonoBehaviour
{
    private BackgroundDatabase backgroundDatabase;
    private VideoPlayer videoPlayer;

    //En fallback, toma un video aletatorio de la base de datos
    [SerializeField] private VideoClip defaultVideo;

    [SerializeField] private float targetAlpha = 0.3f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private bool playOnStart = true;
    private bool isFading = false;

    void Awake()
    {
        backgroundDatabase = Resources.Load<BackgroundDatabase>("BackgroundDatabase");
        videoPlayer = GetComponent<VideoPlayer>();
        if(videoPlayer.targetCamera == null)
            videoPlayer.targetCamera = Camera.main;
    }

    void Start()
    {
        if(playOnStart)
            PlayBackgroundVideo();
    }

    [ContextMenu("Test Play Background Video")]
    public void PlayBackgroundVideo()
    {
        VideoClip clipToPlay = defaultVideo != null ? defaultVideo : backgroundDatabase.GetRandomBackgroundVideo();

        if (clipToPlay != null)
        {
            videoPlayer.clip = clipToPlay;
            videoPlayer.targetCameraAlpha = 0f; // Start fully transparent
            isFading = true;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("No background video found in the database and no default video assigned.");
        }
    }

    void Update()
    {
        if(isFading)
        {
            float alpha = Mathf.Lerp(videoPlayer.targetCameraAlpha, targetAlpha, Time.deltaTime / fadeDuration);
            videoPlayer.targetCameraAlpha = alpha;

            if (Mathf.Abs(videoPlayer.targetCameraAlpha - targetAlpha) < 0.01f)
            {
                videoPlayer.targetCameraAlpha = targetAlpha;
                isFading = false;
            }
        }
    }  
}
