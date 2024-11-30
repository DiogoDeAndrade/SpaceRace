using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine.UIElements;

public class Engine : ToolContainer
{
    [SerializeField] SpriteRenderer reactorSprite;
    [SerializeField] RectTransform  reactorMeter;
    [SerializeField] AudioClip chargeSnd;
    [SerializeField] AudioSource engineThrumAudioSource;

    struct FuelData
    {
        public Sprite sprite;
        public float  ammount;
        public float  energy;
        public Color  color;
    }
    List<FuelData> fuelQueue = new();
    float maxFuelAmmount = 0;
    float fuelAmmount = 0;
    float fuelEnergy = 0;
    Material material;
    Pipe[] pipes;
    float lightIntensity;
    Light2D reactorLight;
    Background background;

    protected override void Start()
    {
        base.Start();

        pipes = FindObjectsByType<Pipe>(FindObjectsSortMode.None);
        reactorSprite.enabled = false;
        material = reactorSprite.material;

        reactorLight = GetComponentInChildren<Light2D>();
        if (reactorLight) lightIntensity = reactorLight.intensity;

        background = FindFirstObjectByType<Background>();
    }

    protected override void Update()
    {
        base.Update();

        if (reactorSprite.enabled)
        {
            fuelAmmount -= Time.deltaTime;

            float p = Mathf.Clamp01(fuelAmmount / maxFuelAmmount);
            float c = Mathf.Max(0, p * 4.0f + 0.5f * Mathf.Sin(Time.time * 5.0f));

            material.SetColor("_EmissiveColor", new Color(c, c, c, 1.0f));
            if (reactorLight) reactorLight.intensity = p;

            if (fuelAmmount < 0)
            {
                reactorSprite.enabled = false;
                fuelEnergy = 0.0f;
            }

            reactorMeter.localScale = new Vector3(1, p, 1);

            if (engineThrumAudioSource)
            {
                if (!engineThrumAudioSource.isPlaying) engineThrumAudioSource.Play();
                engineThrumAudioSource.volume = p;
            }
        }
        else
        {
            if (reactorLight) reactorLight.intensity = 0;
            reactorMeter.localScale = new Vector3(1, 0, 1);
            if (fuelQueue.Count > 0)
            {
                var fuel = fuelQueue.PopFirst();
                reactorSprite.enabled = true;
                reactorSprite.sprite = fuel.sprite;
                reactorSprite.color = fuel.color;
                fuelAmmount = maxFuelAmmount = fuel.ammount;
                fuelEnergy = fuel.energy;
            }

            if ((engineThrumAudioSource) && (engineThrumAudioSource.isPlaying)) engineThrumAudioSource.Stop();
        }

            float deltaRace = 1.0f + fuelEnergy;

        // Account for pipe breaks
        float totalPipe = 0.0f;
        foreach (var pipe in pipes)
        {
            totalPipe += (1 - pipe.damagePercentage);
        }
        totalPipe /= pipes.Length;
        deltaRace *= totalPipe;

        LevelManager.ChangeRace(deltaRace * Time.deltaTime);
        if (background)
        {
            background.SetSpeedScale(deltaRace);
        }
    }

    public override bool HangTool(Player player, Tool tool)
    {
        var fuel = tool.GetComponent<Fuel>();
        if (fuel != null)
        {
            fuelQueue.Add(new FuelData
            {
                sprite = fuel.sprite,
                color = fuel.color,
                ammount = fuel.ammount,
                energy = fuel.energy,
            });
            player.AddScore(fuel.score);
            Destroy(tool.gameObject);

            if (chargeSnd) SoundManager.PlaySound(SoundType.PrimaryFX, chargeSnd);

            return true;
        }

        return false;
    }
}
