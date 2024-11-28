using UnityEngine;

public class ToolDispenser : Interactable
{
    [SerializeField] private Tool       _toolPrefab;
    [SerializeField] private int        _maxCharges;
    [SerializeField] private Transform  _toolPos;

    public Transform toolPos => _toolPos;

    public bool hasCharges => (currentCharges > 0) || (_maxCharges == 0);

    int currentCharges;

    protected override void Start()
    {
        base.Start();

        currentCharges = _maxCharges;
    }

    protected virtual void Update()
    {
        if (anim)
        {
            anim.SetBool("HasCharges", hasCharges);
        }
    }

    public override void Interact(Player player)
    {
        if (!hasCharges)
        {
            return;
        }
        if (player.hasTool)
        {
            return;
        }
        if (_maxCharges > 0) currentCharges--;

        var newTool = Instantiate(_toolPrefab, toolPos.position, toolPos.rotation);

        if (player)
        {
            newTool.Interact(player);
        }
    }
}
