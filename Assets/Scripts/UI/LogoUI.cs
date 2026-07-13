using UnityEngine;

public class LogoUI : MonoBehaviour
{
    [SerializeField] private FeedbackController feedbackController;

    [SerializeField] private float offset;
    [SerializeField] private float everySeconds = 0.5f;

    void Awake()
    {
        StartCoroutine(AnimateLogo());
    }

    private System.Collections.IEnumerator AnimateLogo()
    {

        yield return new WaitForSeconds(offset);

        while (true)
        {
            feedbackController.PlayFeedback();
            yield return new WaitForSeconds(everySeconds);
        }
    }

}
