using UnityEngine;

public class HullBreach : Accident, LevelManager.Force
{
    [SerializeField] float oxygenPerSecond = 10.0f;
    [SerializeField] float attractionStrength = 25;
    [SerializeField] float attractionRange = 200;

    SpriteRenderer spriteRenderer;
    ParticleSystem ps;
    float          defaultEmissionRate;

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        defaultEmissionRate = ps.emission.rateOverTimeMultiplier;

        LevelManager.AddForce(this);
    }

    void OnDestroy()
    {
        LevelManager.RemoveForce(this);
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

        LevelManager.ChangeOxygen(-t * oxygenPerSecond * Time.deltaTime);
    }

    public Vector2 GetForce(Vector2 pos)
    {
        Vector2 direction = transform.position.xy() - pos;
        float dist = direction.magnitude;
        if (dist < 40.0f) return Vector2.zero;

        float distNormalized = Mathf.Clamp01(dist / attractionRange);
        direction /= dist;

        float t = currentDamage / maxDamage;

        return Mathf.Lerp(attractionStrength, 0, distNormalized * t) * direction;
    }
}
