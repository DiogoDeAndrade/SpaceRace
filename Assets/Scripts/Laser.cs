using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Laser : MonoBehaviour
{
    [SerializeField] private float      speed;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private float      laserDamage = 50.0f;
    [SerializeField] private int        friendlyFireScore = 100;
    [SerializeField] private AudioClip  wallHitSnd;
    [SerializeField] private AudioClip  alienHitSnd;
    [SerializeField] private AudioClip  playerHitSnd;

    Rigidbody2D     rb;
    TrailRenderer   trailRenderer;
    Light2D         laserLight;
    Player          _owner;

    public Player owner
    {
        get => _owner;
        set => _owner = value;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = speed * transform.right;
        trailRenderer = GetComponent<TrailRenderer>();
        laserLight = GetComponentInChildren<Light2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we hit an enemy
        var alien = collision.GetComponent<Alien>();
        if (alien != null)
        {
            if (!alien.isVulnerable) return;

            alien.Kill();
            laserLight.FadeOut(0.1f);
            owner.AddScore(alien.killScore);

            if (alienHitSnd) SoundManager.PlaySound(SoundType.PrimaryFX, alienHitSnd, 1.0f, Random.Range(0.75f, 1.25f));
        }
        else
        {
            var player = collision.GetComponent<Player>();
            if (player != null)
            {
                var hs = player.GetComponent<HealthSystem>();
                hs.DealDamage(laserDamage, transform.position);
                laserLight.FadeOut(0.1f);

                if (playerHitSnd) SoundManager.PlaySound(SoundType.PrimaryFX, playerHitSnd, 1.0f, Random.Range(0.75f, 1.25f));

                if (!hs.isAlive)
                {
                    owner.AddScore(friendlyFireScore);
                }
            }
            else
            {
                // Probably wall
                Instantiate(hitPrefab, transform.position, transform.rotation);
                laserLight.FadeOut(1.0f);

                if (wallHitSnd) SoundManager.PlaySound(SoundType.PrimaryFX, wallHitSnd, 1.0f, Random.Range(0.75f, 1.25f));
            }
        }

        trailRenderer.emitting = false;
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 1.0f);
    }
}
