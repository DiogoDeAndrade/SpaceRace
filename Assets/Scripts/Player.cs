using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;

public class Player : MonoBehaviour
{
    [SerializeField] 
    private int            _playerId = 1;
    [SerializeField] 
    private int            maxInventorySlots = 0;
    [SerializeField] 
    private PlayerInput    playerInput;
    [SerializeField] 
    private Transform      interactPoint;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton] 
    private InputControl   interactControl;
    [SerializeField] 
    private float          interactRadius = 2.0f;
    [SerializeField] 
    private LayerMask      interactMask;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton] 
    private InputControl   useToolCtrl;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton] 
    private InputControl   dropToolCtrl;

    public int  playerId => _playerId;

    private Tooltip             tooltip;
    private Item                grabbedItem;
    private List<Pickable>      inventory = new();
    private Tool                currentTool;
    private int                 _score;
    private Animator            animator;
    private Rigidbody2D         rb;
    private MovementPlatformer  movementPlatformer;
    private bool                _isDead;
    private HealthSystem        healthSystem;
    private SpriteEffect        spriteEffect;

    public bool hasTool => (currentTool != null);
    public bool hasInventorySpace => inventory.Count < maxInventorySlots;
    public int  score => _score;
    public bool isDead => healthSystem.isDead;

    void Start()
    {
        if (_playerId >= GameManager.Instance.numPlayers)
        {
            Destroy(gameObject);
            return;
        }

        var pd = GameManager.Instance.GetPlayerData(_playerId);

        CharacterCustomization playerCustomization = GetComponent<CharacterCustomization>();
        if (playerCustomization)
        {
            playerCustomization.SetColors(pd.hairColor, pd.bodyColor);
        }
        InputDevice inputDevice = null;
        if (pd.deviceId != -1)
        {
            Debug.Log("Player device set");
            foreach (var device in InputSystem.devices)
            {
                if (device.deviceId == pd.deviceId)
                {
                    inputDevice = device;
                    break;
                }
            }
            if (inputDevice != null)
            {
                playerInput.SwitchCurrentControlScheme(playerInput.currentControlScheme, inputDevice);
            }
        }
        playerInput.enabled = true;

        tooltip = TooltipManager.CreateTooltip();
        interactControl.playerInput = playerInput;
        dropToolCtrl.playerInput = playerInput;
        useToolCtrl.playerInput = playerInput;

        var playerUIs = FindObjectsByType<PlayerUI>(FindObjectsSortMode.None);
        foreach (var p in playerUIs)
        {
            if (p.player == null)
            {
                p.player = this;
                break;
            }
        }

        spriteEffect = GetComponent<SpriteEffect>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movementPlatformer = GetComponent<MovementPlatformer>();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.onHit += OnHit;
        healthSystem.onHeal += OnHeal;
        healthSystem.onDead += OnDead;
        healthSystem.onRevive += OnRevive;
    }

    bool isKnockbackActive = false;

    IEnumerator OnHitCR(float damage, Vector3 damagePosition)
    {
        isKnockbackActive = true;
        movementPlatformer.enabled = false;
        float s = Mathf.Sign(transform.position.x - damagePosition.x);
        rb.linearVelocityX = s * 30.0f;
        transform.rotation = (s < 0) ? (Quaternion.identity) : (Quaternion.Euler(0, 180, 0));
        animator.SetTrigger("Hit");

        spriteEffect.FlashInvert(0.25f);

        yield return new WaitForSeconds(0.5f);

        if (!_isDead)
        {
            animator.SetTrigger("Reset");
            movementPlatformer.enabled = true;
        }
        isKnockbackActive = false;
    }

    private void FixedUpdate()
    {
        if (_isDead)
        {
            rb.linearVelocityX = 0.0f;
        }
        else if (isKnockbackActive) return;

        Vector2 f = LevelManager.GetForce(transform.position);
        // Project the force in the X axis only
        f.y = 0.0f;
        rb.AddForce(f, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (_isDead) return;

        if (LevelManager.oxygenPercentage <= 0.0f)
        {
            // Asphyxiate
            animator.SetTrigger("Asphyxiate");
            movementPlatformer.SetActive(false);
            healthSystem.SetHealth(0.0f);
            _isDead = true;
        }
        
        Item            interactionItem = null;
        Interactable    interactable = null;

        var colliders = Physics2D.OverlapCircleAll(interactPoint.position, interactRadius, interactMask);
        foreach (var collider in colliders)
        {
            ToolContainer container = collider.GetComponent<ToolContainer>();
            if ((container != null) && (container.hasTool))
            {
                interactionItem = container.tool;
                break;
            }
            Item item = collider.GetComponent<Item>();
            if ((item != null) && (item.canInteract))
            {
                interactionItem = item;
                break;
            }
            Interactable interactableObject = collider.GetComponent<Interactable>();
            if (interactableObject != null)
            {
                interactable = interactableObject;
                break;
            }
        }

        if (hasTool)
        {
            tooltip.SetText("");

            if (currentTool.useMode == Tool.UseMode.Hold)
                currentTool.activeTool = useToolCtrl.IsPressed();
            else
                currentTool.activeTool = useToolCtrl.IsDown();

            if (dropToolCtrl.IsPressed())
            {
                DropTool();
            }
            if ((interactionItem) && (interactControl.IsDown()))
            {
                // Exchange tool
                interactionItem.Interact(this);
            }
        }
        else
        {
            if ((interactionItem == null) && (interactable == null))
            {
                tooltip.SetText("");
            }
            else
            {
                tooltip.SetText((interactable) ? (interactable.displayName) : (interactionItem.displayName));
                tooltip.SetPosition((interactable) ? (interactable.tooltipPosition.position) : (interactionItem.tooltipPosition.position));

                if (interactControl.IsDown())
                {
                    interactionItem?.Interact(this);
                    interactable?.Interact(this);
                }
            }
        }
    }

    public void SetPlayerId(int playerId)
    {
        _playerId = playerId;
    }

    private void OnDrawGizmos()
    {
        if (interactPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(interactPoint.position, interactRadius);
        }
    }

    internal void AddToInventory(Pickable pickable)
    {
        pickable.owner = this;
        inventory.Add(pickable);
    }

    internal void DropTool(Item exchangeTool = null)
    {
        // Check if we're at an empty container that can accept this tool
        var colliders = Physics2D.OverlapCircleAll(interactPoint.position, interactRadius, interactMask);
        foreach (var collider in colliders)
        {
            ToolContainer container = collider.GetComponent<ToolContainer>();
            if ((container != null) && (container.toolDef == currentTool.toolDef))
            {
                if (((container.hasTool) && (container.tool == exchangeTool)) || (!container.hasTool))
                {
                    currentTool.SetContainer(this, container);
                    currentTool = null;
                    return;
                }
            }
        }

        // Didn't find, throw it
        currentTool.Throw(transform.right);

        currentTool = null;
    }

    internal void SetTool(Tool tool)
    {
        tool.transform.SetParent(interactPoint.transform);
        tool.transform.localPosition = Vector3.zero;
        tool.transform.localRotation = Quaternion.identity;
        tool.owner = this;
        currentTool = tool;
    }

    public void AddScore(int delta)
    {
        _score += delta;

        var pd = GameManager.Instance.GetPlayerData(_playerId);
        pd.score = _score;
    }

    public Sprite toolImage => currentTool.toolDef.sprite;
    public float toolCharge => currentTool.chargePercentage;


    private void OnHit(float damage, Vector3 damagePosition)
    {
        StartCoroutine(OnHitCR(damage, damagePosition));        
    }

    private void OnHeal(float healthGain)
    {
        spriteEffect.FlashColor(0.2f, Color.green);
    }

    private void OnDead()
    {
        spriteEffect.FlashInvert(0.25f);

        animator.SetTrigger("Asphyxiate");
        movementPlatformer.SetActive(false);
        _isDead = true;
    }

    private void OnRevive()
    {
        animator.SetTrigger("Reset");
        movementPlatformer.SetActive(true);
        _isDead = false;
    }

    public InputControl GetInteractControl() => interactControl;
}
