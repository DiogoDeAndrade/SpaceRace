using UnityEngine;

public class Pipe : Accident
{
    [SerializeField] private ParticleSystem steamPS;

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

        var emission = steamPS.emission;
        emission.rateOverTime = t * emissionRate;
    }

    public void Break()
    {
        currentDamage = maxDamage;
    }
}
