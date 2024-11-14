using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer>   fires;
    [SerializeField] ParticleSystem         firePS;
    [SerializeField] float                  particlesPerSecond = 10.0f;

    private float   elapsedTime = 0.0f;
    private float   timePerParticle;
    private Color32 color;

    void Start()
    {
        timePerParticle = 1.0f / particlesPerSecond;
        color = new Color32(255, 255, 255, 255);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        
        while (elapsedTime > timePerParticle)
        {
            var where = fires.Random();
            var p = new ParticleSystem.EmitParams();
            p.position = where.transform.position;
            p.velocity = where.transform.up * Random.Range(3.0f, 8.0f);
            firePS.Emit(p, 1);
            elapsedTime -= timePerParticle;
        }
    }
}
