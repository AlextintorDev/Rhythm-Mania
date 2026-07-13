using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "BackgroundDatabase", menuName = "Rhythm Mania/BackgroundDatabase")]
public class BackgroundDatabase : ScriptableObject
{
    //Lista de videos para fondo
    [SerializeField] private VideoClip[] backgroundVideos;

    public VideoClip GetRandomBackgroundVideo()
    {
        if (backgroundVideos.Length == 0)
        {
            Debug.LogWarning("No background videos assigned in the BackgroundDatabase.");
            return null;
        }

        int randomIndex = Random.Range(0, backgroundVideos.Length);
        return backgroundVideos[randomIndex];
    }

    public VideoClip GetBackgroundVideo(int index)
    {
        if (backgroundVideos.Length == 0)
        {
            Debug.LogWarning("No background videos assigned in the BackgroundDatabase.");
            return null;
        }

        if (index < 0 || index >= backgroundVideos.Length)
        {
            Debug.LogWarning($"Requested background video index {index} is out of range. Returning the first video.");
            return backgroundVideos[0];
        }

        return backgroundVideos[index];
    }
}
