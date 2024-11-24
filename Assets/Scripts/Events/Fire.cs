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
    [SerializeField] float                  damageRadius;
    [SerializeField] float                  damage;

    private Light2D         fireLight;
    private float           fireFlickerTimer;     
    private float           elapsedTime = 0.0f;
    private float           timePerParticle;
    private Color32         color;
    private List<Animator>  fireAnimators;

    private Dictionary<HealthSystem, float> firstContact = new();

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

            // Find all players that can be hurt
            var healthSystems = HealthSystem.FindAll(transform.position, damageRadius);
            foreach (var healthSystem in healthSystems)
            {
                if (healthSystem.faction == HealthSystem.Faction.Friendly)   
                {
                    if (firstContact.TryGetValue(healthSystem, out float timeOfFirstContact))
                    {
                        float elapsedTime = (Time.time - timeOfFirstContact);
                        if ((elapsedTime > 1.0f) && (elapsedTime < 5.0f))
                        {
                            healthSystem.DealDamage(damage, transform.position);
                            firstContact.Remove(healthSystem);
                        }
                        else if (elapsedTime > 5.0f)
                        {
                            firstContact[healthSystem] = Time.time;
                        }
                    }
                    else
                    {
                        firstContact.Add(healthSystem, Time.time);
                    }
                }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
