using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LowLightController : MonoBehaviour
{
    //Ejemplo, desaparece en 0.5 segundos
    [SerializeField] private float lightDisppearSpeed = 0.5f;
    private SpriteRenderer sr;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(sr.color.a > 0)
        {
            Color c = sr.color;
            c.a -= Time.deltaTime * lightDisppearSpeed;
            sr.color = c;
        }
    }

    public void SetLightColor(Color color)
    {
        sr.color = color;
    }

}
