using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

public class FeedbackController : MonoBehaviour, IFeedbackPlayer
{
    [SerializeField] private UnityEvent onFeedback = new UnityEvent();
    [SerializeField] private MMF_Player mmFeedback;
    
    public void PlayFeedback()
    {
        if(!IsFeedbackEnabled())
            return;
        
        if (mmFeedback != null)        
            mmFeedback.PlayFeedbacks();
        
        if(onFeedback != null)
            onFeedback.Invoke();
    }   

    public static bool IsFeedbackEnabled() => GameSettings.Instance.UsingGameFeel;
}