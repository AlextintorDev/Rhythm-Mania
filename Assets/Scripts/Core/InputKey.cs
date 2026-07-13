using System.Collections.Generic;
using UnityEngine;

public class InputKey : MonoBehaviour
{
    public List<KeyCode> keys = new List<KeyCode>();
    private List<Note> overlappingNotes = new List<Note>();

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite idleSprite, pressedSprite;
    [SerializeField] private GlowEffectController glowEffect;

    void Update()
    {
        bool isPressed = false;
        foreach (var key in keys)
        {
            if(Input.GetKeyDown(key) && overlappingNotes.Count <= 0)
            {
                AnalyticsRecorder.RecordGhostNote();
            }

            if (Input.GetKey(key))
            {
                isPressed = true;
                if(GameSettings.Instance.UsingGameFeel)
                {
                    glowEffect.TriggerGlow(); 
                }
                break;
            }
        }
        if(GameSettings.Instance.UsingGameFeel)
            spriteRenderer.sprite = isPressed ? pressedSprite : idleSprite;
        else
            spriteRenderer.transform.localPosition = isPressed ? new Vector3(0, 0, 0.15f) : Vector3.zero;

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("[InputKey] InputKey entered trigger with name: " + collision.name);
        if(collision.TryGetComponent(out Note note))
        {
            // Debug.Log("[InputKey] Note detected in InputKey trigger");
            overlappingNotes.Add(note);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Note note))
        {
            overlappingNotes.Remove(note);
        }
    }
}
