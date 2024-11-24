using System.Collections;
using UnityEngine;

public class FuelPellet : Tool
{
    [SerializeField] float          maxStability = 30.0f;
    [SerializeField] AnimationCurve shakeStabilityControl;
    [SerializeField] Gradient       colorStabilityControl;
    [SerializeField] float          maxShake = 2.0f;
    [SerializeField] Transform      gfx;
    [SerializeField] GameObject     explosionPrefab;

    float           stability;
    SpriteRenderer  spriteRenderer;
    Collider2D      mainCollider;

    protected override void Start()
    {
        base.Start();

        stability = maxStability;
        spriteRenderer = gfx.GetComponent<SpriteRenderer>();

        mainCollider = GetComponent<Collider2D>();
        StartCoroutine(ActivateColliderAfterTime(0.2f));
    }

    IEnumerator ActivateColliderAfterTime(float time)
    {
        mainCollider.enabled = false;
        yield return new WaitForSeconds(time);
        mainCollider.enabled = true;
    }

    protected override void Update()
    {
        base.Update();

        if (stability > 0.0f)
        {
            stability -= Time.deltaTime;

            float p = Mathf.Clamp01(stability / maxStability);
            float shakeStrength = shakeStabilityControl.Evaluate(p);
            gfx.localPosition = Random.insideUnitCircle * shakeStrength * maxShake;
            Color color = colorStabilityControl.Evaluate(p);
            spriteRenderer.color = color;

            if (p <= 0.0f)
            {
                // Explode
                Instantiate(explosionPrefab, transform.position, transform.rotation);
                Destroy(gameObject, 0.1f);
            }
        }
    }
}
