using NaughtyAttributes;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShipLight : MonoBehaviour
{
    private Light2D         light2d;
    private float           originalLightIntensity;
    private Color           originalLightColor;
    private SpriteRenderer  spriteRenderer;
    private Material        material;
    private Color           originalEmissive;

    void Start()
    {
        light2d = GetComponent<Light2D>();
        originalLightIntensity = light2d.intensity;
        originalLightColor = light2d.color;
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        originalEmissive = material.GetColor("_EmissiveColor");
    }

    [Button("Turn Off")]
    public void TurnOff()
    {
        light2d.intensity = 0.1f;
        light2d.color = Color.red;
        material.SetColor("_EmissiveColor", Color.red * 3.0f);
    }

    [Button("Turn On")]
    public void TurnOn()
    {
        //light2d.enabled = true;
        light2d.intensity = originalLightIntensity;
        light2d.color = originalLightColor;
        material.SetColor("_EmissiveColor", originalEmissive);
    }
}
