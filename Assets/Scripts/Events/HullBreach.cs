using UnityEngine;

public class HullBreach : Accident
{
    [SerializeField] float oxygenPerSecond = 10.0f;

    SpriteRenderer spriteRenderer;
    ParticleSystem ps;
    float          defaultEmissionRate;

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        defaultEmissionRate = ps.emission.rateOverTimeMultiplier;
    }

    protected override void Update()
    {
        base.Update();

        float t = currentDamage / maxDamage;

        var emission = ps.emission;
        emission.rateOverTimeMultiplier = defaultEmissionRate * t;

        if (t < 0.05f)
        {
            if (ps.particleCount == 0)
            {
                Destroy(gameObject);
            }
        }

        spriteRenderer.color = spriteRenderer.color.ChangeAlpha(t);

        GameManager.ChangeOxygen(-t * oxygenPerSecond * Time.deltaTime);
    }
}
