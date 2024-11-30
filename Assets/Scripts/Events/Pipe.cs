using UnityEngine;
using UnityEngine.Audio;

public class Pipe : Accident
{
    [SerializeField] private ParticleSystem steamPS;
    [SerializeField] private AudioSource audioSource;

    float emissionRate;

    protected override void Start()
    {
        base.Start();

        var emission = steamPS.emission;
        emissionRate = emission.rateOverTime.constant;
        currentDamage = 0;
    }

    protected override void Update()
    {
        base.Update();

        float t = currentDamage / maxDamage;

        if (t > 0.0f)
        {
            if (!audioSource.isPlaying) audioSource.Play();
            audioSource.volume = t;
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }

        var emission = steamPS.emission;
        emission.rateOverTime = t * emissionRate;
    }

    public void Break()
    {
        currentDamage = maxDamage;
    }
}
