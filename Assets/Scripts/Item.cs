using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField]
    private string _displayName;
    [SerializeField] Transform _tooltipPosition;

    protected Rigidbody2D rb;
    protected Collider2D mainCollider;
    private Player _owner = null;

    public Player owner
    {
        get => _owner;

        set
        {
            if ((value != null) && ((_owner != value) && (_owner != null)))
            {
                Debug.LogWarning($"Tried to pickup item that belongs to someone else {_owner.name}!");
                return;
            }
            _owner = value;
        }
    }
    public string displayName => _displayName;
    public Transform tooltipPosition => (_tooltipPosition != null) ? (_tooltipPosition) : (transform);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual bool canInteract => false;

    public abstract bool Interact(Player player);

    protected void SetPhysics(bool active)
    {
        rb.simulated = active;
        mainCollider.enabled = active;
    }
}
