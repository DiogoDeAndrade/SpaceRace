using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Laser : MonoBehaviour
{
    [SerializeField] private float      speed;
    [SerializeField] private GameObject hitPrefab;

    Rigidbody2D     rb;
    TrailRenderer   trailRenderer;
    Light2D         laserLight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = speed * transform.right;
        trailRenderer = GetComponent<TrailRenderer>();
        laserLight = GetComponentInChildren<Light2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        trailRenderer.emitting = false;
        rb.linearVelocity = Vector2.zero;
        laserLight.FadeOut(1.0f);
        Instantiate(hitPrefab, transform.position, transform.rotation);
        Destroy(gameObject, 1.0f);
    }
}
