using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fire : Accident
{
    [SerializeField] List<SpriteRenderer>   fires;
    [SerializeField] ParticleSystem         firePS;
    [SerializeField] float                  particlesPerSecond = 10.0f;
    [SerializeField] float                  oxygenPerSecond = 10.0f;
    [SerializeField] Gradient               fireFlickerColor;
    [SerializeField] float                  fireFlickerSpeed;

    private Light2D         fireLight;
    private float           fireFlickerTimer;     
    private float           elapsedTime = 0.0f;
    private float           timePerParticle;
    private Color32         color;
    private List<Animator>  fireAnimators;

    protected override void Start()
    {
        base.Start();

        fireLight = GetComponent<Light2D>();
        timePerParticle = 1.0f / particlesPerSecond;
        color = new Color32(255, 255, 255, 255);

        fireAnimators = new();
        for (int i = 0; i < fires.Count; i++)
        {
            fireAnimators.Add(fires[i].GetComponent<Animator>());
        }
    }

    protected override void Update()
    {
        base.Update();

        float t = currentDamage / maxDamage;

        if (t > 0.05f)
        {
            elapsedTime += Time.deltaTime;

            while (elapsedTime > timePerParticle)
            {
                var where = fires.Random();
                if (where.enabled)
                {
                    var p = new ParticleSystem.EmitParams();
                    p.position = where.transform.position;
                    p.velocity = where.transform.up * Random.Range(3.0f, 8.0f);
                    firePS.Emit(p, 1);
                }
                elapsedTime -= timePerParticle;
            }
        }
        else
        {
            if (firePS.particleCount == 0)
            {
                Destroy(gameObject);
            }
        }
        fireFlickerTimer += Time.deltaTime * fireFlickerSpeed;
        while (fireFlickerTimer > 1) fireFlickerTimer -= 1.0f;
        fireLight.color = fireFlickerColor.Evaluate(fireFlickerTimer);
        fireLight.intensity = fireLight.intensity + (t - fireLight.intensity) * 0.05f;

        int numberOfActiveFires = Mathf.CeilToInt(t * fires.Count);
        for (int i = 0; i < fires.Count; i++)
        {
            fireAnimators[i].SetBool("Burning", (i < numberOfActiveFires));
        }

        GameManager.ChangeOxygen(-t * oxygenPerSecond * Time.deltaTime);
    }

    protected override void Complete(Player player)
    {
        base.Complete(player);
    }
}
