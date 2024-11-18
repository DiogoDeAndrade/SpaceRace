using UnityEngine;

public class Tool : Item
{
    [SerializeField] ToolDef        _toolDef;
    [SerializeField] LayerMask      accidentMask;
    [SerializeField] ParticleSystem usePS;
    [SerializeField] Transform      toolPoint;
    [SerializeField] float          toolRadius = 5.0f;
    [SerializeField] float          maxCharge = 5.0f;
    [SerializeField] Canvas         chargeUI;
    [SerializeField] RectTransform  chargeMeter;

    private bool    _toolActive = false;
    private ToolContainer currentContainer;
    private float currentCharge;

    public bool activeTool
    {
        get => _toolActive;
        set => _toolActive = value;
    }
    public ToolDef toolDef => _toolDef;

    public bool hasCharge => chargePercentage > 0.0f;
    public float chargePercentage => (maxCharge > 0.0f) ? (currentCharge / maxCharge) : (1.0f);

    protected override void Start()
    {
        base.Start();

        currentCharge = maxCharge;
        _toolActive = false;
    }

    public override bool canInteract => true;
    public override bool Interact(Player player)
    {
        // If this tool is on a container, clear the container first
        // We need to do this before we let drop the current tool
        // because otherwise the container can't store the tool
        if (currentContainer)
        {
            SetContainer(null);
        }
        if (player.hasTool)
        {
            player.DropTool(this);
        }

        player.SetTool(this);

        SetPhysics(false);

        return true;
    }

    public void Throw(Vector2 direction)
    {
        transform.SetParent(null, true);
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
                    accident.Fix(owner, 1.0f);
                }
            }

            currentCharge = Mathf.Max(0, currentCharge - Time.deltaTime);
        }

        chargeUI.gameObject.SetActive((currentContainer != null) && (chargePercentage < 1.0f));
        chargeMeter.localScale = new Vector2(chargePercentage, 1);
    }

    public void Charge(float amount)
    {
        if (maxCharge > 0.0f)
        {
            currentCharge = Mathf.Clamp(currentCharge + amount, 0.0f, maxCharge);
        }
    }

    public void SetContainer(ToolContainer container)
    {
        var prevContainer = currentContainer;
        currentContainer = container;

        if (currentContainer)
        {
            SetPhysics(false);
            transform.SetParent(container.toolPos);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            prevContainer?.HangTool(null);
            currentContainer.HangTool(this);
        }
        else
        {
            transform.SetParent(null);
            SetPhysics(true);            
            prevContainer?.HangTool(null);
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
