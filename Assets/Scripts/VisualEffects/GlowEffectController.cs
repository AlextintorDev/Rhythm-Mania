using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GlowEffectController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float disappearSpeed = 1f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(spriteRenderer.color.a > 0f)
            ChangeAlpha(spriteRenderer.color.a - disappearSpeed * Time.deltaTime);
    }

    public void TriggerGlow()
    {
        ChangeAlpha(1f);
    }

    void ChangeAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = Mathf.Clamp01(alpha);
        spriteRenderer.color = color;
    }
}
