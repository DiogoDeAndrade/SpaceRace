using NaughtyAttributes;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
    [SerializeField] private ColorPalette   originalPalette;
    [SerializeField] private bool           randomizeAtStart;
    [SerializeField] private Color          hairColor = Color.magenta;
    [SerializeField] private Color          flightsuitColor = Color.green;

    ColorPalette modifiedPalette;

    private void Start()
    {
        if (randomizeAtStart)
        {
            hairColor = Color.HSVToRGB(Random.Range(0.0f, 1.0f), Random.Range(0.75f, 1.0f), Random.Range(0.75f, 1.0f));
            flightsuitColor = Color.HSVToRGB(Random.Range(0.0f, 1.0f), Random.Range(0.25f, 0.5f), Random.Range(0.5f, 0.75f));
        }

        UpdateCharacter();
    }

    [Button("Update Character")]    
    void UpdateCharacter()
    {
        modifiedPalette = BuildPalette(originalPalette, hairColor, flightsuitColor);

        SpriteEffect effect = GetComponent<SpriteEffect>();
        effect.SetRemap(modifiedPalette);
    }

    public void SetColors(Color hairColor, Color flightsuitColor)
    {
        this.hairColor = hairColor;
        this.flightsuitColor = flightsuitColor;
        randomizeAtStart = false;

        UpdateCharacter();
    }

    public static ColorPalette BuildPalette(ColorPalette originalPalette, Color hairColor, Color flightsuitColor)
    {
        var modifiedPalette = originalPalette.Clone();

        Color.RGBToHSV(hairColor, out float h, out float s, out float v);
        modifiedPalette.SetColor(13, Color.HSVToRGB(h, s, v * 0.3f));
        modifiedPalette.SetColor(14, Color.HSVToRGB(h, s, v * 0.6f));
        modifiedPalette.SetColor(15, Color.HSVToRGB(h, s, v));

        Color.RGBToHSV(flightsuitColor, out h, out s, out v);
        modifiedPalette.SetColor(0, Color.HSVToRGB(h, s, v * 0.25f));
        modifiedPalette.SetColor(1, Color.HSVToRGB(h, s, v * 0.50f));
        modifiedPalette.SetColor(2, Color.HSVToRGB(h, s, v * 0.75f));
        modifiedPalette.SetColor(3, Color.HSVToRGB(h, s, v));
        modifiedPalette.SetColor(4, Color.HSVToRGB(h, s * 0.5f, v * 0.15f));

        modifiedPalette.RefreshCache();

        return modifiedPalette;
    }
}
