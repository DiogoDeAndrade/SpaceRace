using UnityEngine;

public class Tool : Item
{
    [SerializeField] ToolDef        _toolDef;
    [SerializeField] LayerMask      accidentMask;
    [SerializeField] ParticleSystem usePS;
    [SerializeField] Transform      toolPoint;
    [SerializeField] float          toolRadius = 5.0f;
    [SerializeField] float          maxCharge = 5.0f;

    private bool _toolActive = false;
    private float currentCharge;

    public bool activeTool
    {
        get => _toolActive;
        set => _toolActive = value;
    }
    public ToolDef toolDef => _toolDef;

    public bool hasCharge => (currentCharge > 0.0f);
    public float charge => (maxCharge > 0.0f) ? (currentCharge / maxCharge) : (1.0f);

    protected override void Start()
    {
        base.Start();

        currentCharge = maxCharge;
        _toolActive = false;
    }

    public override bool canInteract => true;
    public override bool Interact(Player player)
    {
        if (player.hasTool)
        {
            player.DropTool();
        }

        player.SetTool(this);

        SetPhysics(false);

        return true;
    }

    public void Throw(Vector2 direction)
    {
        transform.SetParent(null);
        owner = null;
        _toolActive = false;

        SetPhysics(true);

        rb.linearVelocity = direction * 100.0f + Vector2.up * 75.0f;
    }

    private void Update()
    {
        if (usePS)
        {
            var emission = usePS.emission;
            emission.enabled = (_toolActive) && (hasCharge);
        }
        if ((_toolActive) && (currentCharge > 0.0f))
        {
            Vector3 toolPos = transform.position;
            if (toolPoint) toolPos = toolPoint.position;

            var colliders = Physics2D.OverlapCircleAll(toolPos, toolRadius, accidentMask);
            foreach (var collider in colliders)
            {
                Accident accident = collider.GetComponent<Accident>();
                if ((accident != null) && (accident.fixTool == toolDef))
                {
                    accident.Fix(1.0f);
                }
            }

            currentCharge = Mathf.Max(0, currentCharge - Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 toolPos = transform.position;
        if (toolPoint) toolPos = toolPoint.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(toolPos, toolRadius);
    }
}
