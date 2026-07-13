using UnityEngine;

public class LightsManager : MonoBehaviour
{
    [SerializeField] private LightController[] lights;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color[] lightColors;
    [SerializeField] private LowLightController lowLightController;
    [SerializeField] private Color onHitLowLightColor = new Color(1f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private Color onMissLowLightColor = new Color(0.5f, 0.5f, 1f, 0.5f);



    void Awake()
    {
        if(!GameSettings.Instance.UsingGameFeel)
        {
            ChangeLightsColor(new Color[] { defaultColor });
            //Manejar apagado de efectos en luces, cambiar por estandar
            return;
        }

        ChangeLightsColor(lightColors);
        GameEventBus.Subscribe<HitEvent>(OnHitEvent);
    }

    void OnDisable()
    {
        GameEventBus.Unsubscribe<HitEvent>(OnHitEvent);
    }

    private void OnHitEvent(HitEvent hitEvent)
    {
        bool isHit = hitEvent.Success;
        if(!GameSettings.Instance.UsingGameFeel)
                    return;
        lowLightController.SetLightColor(isHit ? onHitLowLightColor : onMissLowLightColor);
    }

    private void ChangeLightsColor(Color[] color)
    {
        for(int i = 0; i < lights.Length; i++)
        {
            lights[i].ApplyColor(color[Random.Range(0, color.Length)]);
        }
    }

    #if UNITY_EDITOR
    
    [ContextMenu("Test Lights Color")]
    public void TestLightsColor()
    {
        ChangeLightsColor(lightColors);
    }

    [ContextMenu("Test Default Light Color")]
    public void TestDefaultLightColor()
    {
        ChangeLightsColor(new Color[] { defaultColor });
    }
    #endif
}
