using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private string     _displayName;
    [SerializeField] private Transform  _tooltipPosition;

    public string displayName => _displayName;
    public Transform tooltipPosition => (_tooltipPosition != null) ? (_tooltipPosition) : (transform);

    protected SpriteRenderer    spriteRenderer;
    protected Animator          anim;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public abstract void Interact(Player player);
}
