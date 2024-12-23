using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Tool : Item
{
    public enum UseMode { Hold, Single, SingleIfUsed };

    [SerializeField] protected ToolDef        _toolDef;
    [SerializeField] protected LayerMask      accidentMask;
    [SerializeField] protected ParticleSystem usePS;
    [SerializeField] protected Light2D        useLight;
    [SerializeField] protected Transform      toolPoint;
    [SerializeField] protected float          toolRadius = 5.0f;
    [SerializeField] protected UseMode        _useMode = UseMode.Hold;
    [SerializeField] protected float          maxCharge = 5.0f;
    [SerializeField] protected float          chargeCost = 1.0f;
    [SerializeField] protected Canvas         chargeUI;
    [SerializeField] protected RectTransform  chargeMeter;
    [SerializeField] protected bool           destroyWhenNoCharge = false;
    [SerializeField] protected int            scoreOnNoCharge = 0;
    [SerializeField] protected AudioSource    activeToolAudioSource;
    [SerializeField] protected AudioClip      noChargeSnd;

    protected bool    _toolActive = false;
    protected ToolContainer currentContainer;
    protected float currentCharge;
    protected Animator animator;
    protected float noChargeSoundTimer;

    public bool activeTool
    {
        get => _toolActive;
        set => _toolActive = value;
    }
    public ToolDef toolDef => _toolDef;
    public UseMode useMode => _useMode;

    public bool hasCharge => chargePercentage > 0.0f;
    public float chargePercentage => (maxCharge > 0.0f) ? (currentCharge / maxCharge) : (1.0f);

    protected override void Start()
    {
        base.Start();

        currentCharge = maxCharge;
        _toolActive = false;
        animator = GetComponent<Animator>();
    }

    public override bool canInteract => true;
    public override bool Interact(Player player)
    {
        // If this tool is on a container, clear the container first
        // We need to do this before we let drop the current tool
        // because otherwise the container can't store the tool
        if (currentContainer)
        {
            SetContainer(player, null);
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

    protected virtual void Update()
    {
        if (usePS)
        {
            var emission = usePS.emission;
            emission.enabled = (_toolActive) && (hasCharge);
        }
        if (useLight)
        {
            useLight.enabled = (_toolActive) && (currentCharge > 0.0f);
        }
        if (animator)
        {
            animator.SetBool("Use", _toolActive);
        }
        if (_toolActive)
        {
            if (currentCharge > 0.0f)
            {
                if ((activeToolAudioSource) && (!activeToolAudioSource.isPlaying))
                {
                    activeToolAudioSource.Play();
                }

                Vector3 toolPos = transform.position;
                if (toolPoint) toolPos = toolPoint.position;

                bool toolUsed = false;
                var colliders = Physics2D.OverlapCircleAll(toolPos, toolRadius, accidentMask);
                foreach (var collider in colliders)
                {
                    if (RunTool(collider))
                    {
                        toolUsed = true;
                    }
                }

                if (_useMode == UseMode.Hold)
                    currentCharge = Mathf.Max(0, currentCharge - Time.deltaTime * chargeCost);
                else if (_useMode == UseMode.Single)
                    currentCharge = Mathf.Max(0, currentCharge - chargeCost);
                else if ((_useMode == UseMode.SingleIfUsed) && (toolUsed))
                    currentCharge = Mathf.Max(0, currentCharge - chargeCost);

                if ((currentCharge == 0.0f) && (destroyWhenNoCharge))
                {
                    owner.AddScore(scoreOnNoCharge);
                    owner.DropTool(this);
                    Destroy(gameObject);
                }
            }
            else
            {
                if ((activeToolAudioSource) && (activeToolAudioSource.isPlaying)) activeToolAudioSource.Stop();

                if (noChargeSnd)
                {
                    if ((Time.time - noChargeSoundTimer) > 1.0f)
                    {
                        SoundManager.PlaySound(SoundType.PrimaryFX, noChargeSnd, 1.0f, Random.Range(0.75f, 1.25f));
                        noChargeSoundTimer = Time.time;
                    }
                }
            }
        }
        else
        {
            if ((activeToolAudioSource) && (activeToolAudioSource.isPlaying)) activeToolAudioSource.Stop();
        }

        if ((chargeUI) && (chargeMeter))
        {
            chargeUI.gameObject.SetActive((currentContainer != null) && (chargePercentage < 1.0f));
            chargeMeter.localScale = new Vector2(chargePercentage, 1);
        }
    }

    public void Charge(float amount)
    {
        if (maxCharge > 0.0f)
        {
            currentCharge = Mathf.Clamp(currentCharge + amount, 0.0f, maxCharge);
        }
    }

    protected virtual bool RunTool(Collider2D collider)
    {
        Accident accident = collider.GetComponent<Accident>();
        if ((accident != null) && (accident.fixTool == toolDef))
        {
            accident.Fix(owner, 1.0f);
            return true;
        }

        return false;
    }

    public void SetContainer(Player player, ToolContainer container)
    {
        var prevContainer = currentContainer;
        currentContainer = container;

        if (currentContainer)
        {
            SetPhysics(false);
            transform.SetParent(container.toolPos);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            prevContainer?.HangTool(player, null);
            currentContainer.HangTool(player, this);
        }
        else
        {
            transform.SetParent(null);
            SetPhysics(true);            
            prevContainer?.HangTool(player,null);
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
