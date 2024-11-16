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

    private Tooltip         tooltip;
    private Item            grabbedItem;
    private List<Pickable>  inventory = new();
    private Tool            currentTool;
    
    public bool hasTool => (currentTool != null);
    public bool hasInventorySpace => inventory.Count < maxInventorySlots;

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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Item interactionItem = null;

        var colliders = Physics2D.OverlapCircleAll(interactPoint.position, interactRadius, interactMask);
        foreach (var collider in colliders)
        {
            Item item = collider.GetComponent<Item>();
            if ((item != null) && (item.canInteract))
            {
                interactionItem = item;
                tooltip.SetText(item.displayName);
                tooltip.SetPosition(item.tooltipPosition.position);
                break;
            }
        }
        if (interactionItem == null)
        {
            tooltip.SetText("");
        }
        else
        {
            if (interactControl.IsDown())
            {
                interactionItem.Interact(this);
            }
        }

        if (hasTool)
        {
            currentTool.activeTool = useToolCtrl.IsPressed();

            if (dropToolCtrl.IsPressed())
            {
                DropTool();
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

    internal void DropTool()
    {
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

    public Sprite toolImage => currentTool.toolDef.sprite;
    public float toolCharge => currentTool.charge;
}
