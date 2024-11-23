using UnityEngine;

public class FuelProducer : Interactable
{
    [SerializeField] private Transform      wheel;
    [SerializeField] private float          maxAngularSpeed = 180;
    [SerializeField] private float          angularAcceleration = 10;
    [SerializeField] private float          angularDrag = 5;
    [SerializeField] private float          cooldown = 0.1f;
    [SerializeField] private float          shakeDuration = 0.1f;
    [SerializeField] private float          shakeStrenght = 4.0f;
    [SerializeField] private RectTransform  meterUI;
    [SerializeField] private float          turnsToPellet = 10;
    [SerializeField] private Rigidbody2D    pelletPrefab;
    [SerializeField] private Transform      spawnPoint;

    float angularSpeed = 0.0f;
    float wheelAngle;
    float cooldownTimer;
    float shakeTimer;
    float progress;
    Vector3 wheelPos;

    protected override void Start()
    {
        base.Start();

        wheelPos = wheel.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            Vector3 offset = Random.insideUnitCircle * shakeStrenght;
            wheel.localPosition = wheelPos + offset;
        }
        else
        {
            wheel.localPosition = wheelPos;
        }

        wheelAngle += angularSpeed * Time.deltaTime;
        wheel.localRotation = Quaternion.Euler(0, 0, -wheelAngle);

        progress += angularSpeed * Time.deltaTime;
        float maxProgress = turnsToPellet * 360.0f;

        float p = Mathf.Clamp01(progress / maxProgress);
        meterUI.localScale = new Vector3(p, 1, 1);
        if (p >= 1.0f)
        {
            // Produce pellet, drop it
            progress -= maxProgress;

            var newPellet = Instantiate(pelletPrefab, spawnPoint.position, spawnPoint.rotation);
            newPellet.linearVelocity = -transform.right * 10.0f - transform.up * 10.0f;
        }

        angularSpeed = Mathf.Max(0, angularSpeed - angularDrag * Time.deltaTime);
    }

    public override void Interact(Player player)
    {
        if (cooldownTimer > 0) return;

        angularSpeed = Mathf.Min(angularSpeed + angularAcceleration, maxAngularSpeed);

        cooldownTimer = cooldown;
        shakeTimer = shakeDuration;
    }
}
