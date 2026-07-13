using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private GameObject lightObject;
    [Range(0f, 90f), SerializeField] private float rotationRange = 30f;
    [Range(0f, 10f), SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private List<Color> lightColors;

    private float centerAngle;
    private float timeOffset;

    void Awake()
    {
        centerAngle = lightObject.transform.eulerAngles.z;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float angle = centerAngle + Mathf.Sin(Time.time * rotationSpeed + timeOffset) * rotationRange;

        Vector3 euler = lightObject.transform.eulerAngles;
        euler.z = angle;
        lightObject.transform.eulerAngles = euler;
    }

    public void ApplyColor(Color color)
    {
        SpriteRenderer sr = lightObject.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }
}
