using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class NitroProducer : ToolContainer
{
    [SerializeField] int            score;
    [SerializeField] SpriteRenderer nitroSprite;
    [SerializeField] SpriteRenderer leftSide;
    [SerializeField] SpriteRenderer rightSide;
    [SerializeField] Gradient       overloadGradient;
    [SerializeField] float          overloadTime = 5.0f;
    [SerializeField] float          overloadShake = 1.0f;
    [SerializeField] GameObject     explosionPrefab;
    [SerializeField] float          maxNitroEnergy = 10.0f;
    [SerializeField] GameObject     nitroPrefab;
    [SerializeField] AudioSource    activeAudioSource;
    [SerializeField] AudioClip      pelletCreateSnd;

    ParticleSystem leftPS;
    ParticleSystem rightPS;
    Light2D        purpleLight;
    float          leftTimer;
    float          rightTimer;
    Vector2        leftPos;
    Vector2        rightPos;
    float          nitroEnergy;

    protected override void Start()
    {
        base.Start();

        leftPS = leftSide.GetComponentInChildren<ParticleSystem>();
        leftPos = leftSide.transform.position;
        rightPS = rightSide.GetComponentInChildren<ParticleSystem>();
        rightPos = rightSide.transform.position;

        EnableEmitter(leftPS, false);
        EnableEmitter(rightPS, false);

        purpleLight = GetComponentInChildren<Light2D>();
    }

    protected override void Update()
    {
        base.Update();

        float volume = 0.0f;

        purpleLight.intensity = 0.0f;
        if (leftSide.enabled)
        {
            leftTimer += Time.deltaTime;
            
            float p = Mathf.Clamp01(leftTimer / overloadTime);

            leftSide.color = overloadGradient.Evaluate(p);
            leftSide.transform.position = leftPos + Random.insideUnitCircle * overloadShake * p;

            if (p >= 1.0f)
            {
                Instantiate(explosionPrefab, leftPos, transform.rotation);
                leftSide.enabled = false;
                EnableEmitter(leftPS, false);
            }

            purpleLight.intensity += 2.5f;
            volume = 0.5f;
        }
        if (rightSide.enabled)
        {
            rightTimer += Time.deltaTime;

            float p = Mathf.Clamp01(rightTimer / overloadTime);

            rightSide.color = overloadGradient.Evaluate(p);
            rightSide.transform.position = rightPos + Random.insideUnitCircle * overloadShake * p;

            if (p >= 1.0f)
            {
                Instantiate(explosionPrefab, rightPos, transform.rotation);
                rightSide.enabled = false;
                EnableEmitter(rightPS, false);
            }

            purpleLight.intensity += 2.5f;
            volume = 0.5f;
        }

        if ((leftSide.enabled) && (rightSide.enabled))
        {
            nitroEnergy += Time.deltaTime;
            if (nitroEnergy >= maxNitroEnergy)
            {
                nitroSprite.color = nitroSprite.color.ChangeAlpha(0.0f);
                nitroEnergy = 0.0f;
                rightSide.enabled = false;
                EnableEmitter(rightPS, false);
                leftSide.enabled = false;
                EnableEmitter(leftPS, false);

                Instantiate(nitroPrefab, transform.position, transform.rotation);

                if (pelletCreateSnd)
                {
                    SoundManager.PlaySound(SoundType.PrimaryFX, pelletCreateSnd, 1.0f, 0.5f);
                }
            }
        }

        if (activeAudioSource)
        {
            if (volume > 0.0f)
            {
                if (!activeAudioSource.isPlaying) activeAudioSource.Play();
                activeAudioSource.volume = volume;
            }
            else
            {
                if (activeAudioSource.isPlaying) activeAudioSource.Stop();
            }
        }

        float nitroEnergyPercentage = nitroEnergy / maxNitroEnergy;
        nitroSprite.color = nitroSprite.color.ChangeAlpha(nitroEnergyPercentage);
    }

    public override bool HangTool(Player player, Tool tool)
    {
        var fuel = tool.GetComponent<Fuel>();
        if (fuel != null)
        {
            if (!leftSide.enabled)
            {
                leftSide.enabled = true;
                leftSide.sprite = fuel.sprite;
                leftSide.color = fuel.color;
                leftTimer = 0.0f;
                EnableEmitter(leftPS, true);
            }
            else if (!rightSide.enabled)
            {
                rightSide.enabled = true;
                rightSide.sprite = fuel.sprite;
                rightSide.color = fuel.color;
                rightTimer = 0.0f;
                leftTimer = leftTimer * 0.5f;
                EnableEmitter(rightPS, true);
            }
            else
            {
                return false;
            }

            player.AddScore(score);
            Destroy(tool.gameObject);
            return true;
        }
        
        return false;
    }

    void EnableEmitter(ParticleSystem ps, bool enable)
    {
        var emission = ps.emission;
        emission.enabled = enable;
    }
}
