using UnityEngine;

public class ToolContainer : MonoBehaviour
{
    [SerializeField] private ToolDef    _toolDef;
    [SerializeField] private Transform  _toolPos;
    [SerializeField] private float      chargeSpeed = 2.0f;
    [SerializeField] private AudioClip  grabItemSnd;

    public Transform toolPos => _toolPos;
    public ToolDef toolDef => _toolDef;

    Tool containedTool;

    public bool hasTool => containedTool != null;
    public Tool tool => containedTool;

    protected virtual void Start()
    {
        var ct = GetComponentInChildren<Tool>();
        if (ct != null)
        {
            ct.SetContainer(null, this);
            if (_toolDef == null)
            {
                _toolDef = ct.toolDef;
            }
        }
    }

    protected virtual void Update()
    {
        if (containedTool != null)
        {
            containedTool.transform.localPosition = Vector3.zero;
            containedTool.transform.localRotation = Quaternion.identity;
            containedTool.Charge(chargeSpeed * Time.deltaTime);
        }
    }

    public virtual bool HangTool(Player player, Tool tool)
    {
        if ((tool == null) && (containedTool != null))
        {
            if (grabItemSnd) SoundManager.PlaySound(SoundType.PrimaryFX, grabItemSnd);
            containedTool = null;
            return true;
        }

        if (tool.toolDef != _toolDef) return false;

        containedTool = tool;
        if (grabItemSnd) SoundManager.PlaySound(SoundType.PrimaryFX, grabItemSnd);

        return true;
    }
}
