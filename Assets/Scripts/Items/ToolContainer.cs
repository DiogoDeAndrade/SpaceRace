using UnityEngine;

public class ToolContainer : MonoBehaviour
{
    [SerializeField] private ToolDef    _toolDef;
    [SerializeField] private Transform  _toolPos;
    [SerializeField] private float      chargeSpeed = 2.0f;

    public Transform toolPos => _toolPos;
    public ToolDef toolDef => _toolDef;

    Tool containedTool;

    public bool hasTool => containedTool != null;
    public Tool tool => containedTool;

    void Start()
    {
        var ct = GetComponentInChildren<Tool>();
        if (ct != null)
        {
            ct.SetContainer(this);
            if (_toolDef == null)
            {
                _toolDef = ct.toolDef;
            }
        }
    }

    void Update()
    {
        if (containedTool != null)
        {
            containedTool.transform.localPosition = Vector3.zero;
            containedTool.transform.localRotation = Quaternion.identity;
            containedTool.Charge(chargeSpeed * Time.deltaTime);
        }
    }

    public bool HangTool(Tool tool)
    {
        if (tool == null)
        {
            containedTool = null;
            return true;
        }

        if (tool.toolDef != _toolDef) return false;

        containedTool = tool;

        return true;
    }
}
