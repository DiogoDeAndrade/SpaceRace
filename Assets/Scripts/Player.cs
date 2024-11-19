using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public bool hasTool => (currentTool != null);
    public bool hasInventorySpace => inventory.Count < maxInventorySlots;
    public int  score => _score;
    public bool isDead => _isDead;

    void Start()
    {
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

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movementPlatformer = GetComponent<MovementPlatformer>();
    }

    private void FixedUpdate()
    {
        Vector2 f = GameManager.GetForce(transform.position);
        // Project the force in the X axis only
        f.y = 0.0f;
        rb.AddForce(f, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (_isDead) return;

        if (GameManager.oxygenPercentage <= 0.0f)
        {
            // Asphyxiate
            animator.SetTrigger("Asphyxiate");
            movementPlatformer.SetActive(false);
            _isDead = true;
        }

        Item interactionItem = null;

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
        }

        if (hasTool)
        {
            tooltip.SetText("");

            currentTool.activeTool = useToolCtrl.IsPressed();

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
            if (interactionItem == null)
            {
                tooltip.SetText("");
            }
            else
            {
                tooltip.SetText(interactionItem.displayName);
                tooltip.SetPosition(interactionItem.tooltipPosition.position);

                if (interactControl.IsDown())
                {
                    interactionItem.Interact(this);
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
                    currentTool.SetContainer(container);
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
    }

    public Sprite toolImage => currentTool.toolDef.sprite;
    public float toolCharge => currentTool.chargePercentage;
}
