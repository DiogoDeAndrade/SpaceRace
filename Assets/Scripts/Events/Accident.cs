using System;
using UnityEngine;

public class Accident : MonoBehaviour
{
    [SerializeField] protected float    maxDamage = 100.0f;
    [SerializeField] protected float    fixPerSecond = 25.0f;
    [SerializeField] protected ToolDef  _fixTool;

    public ToolDef fixTool => _fixTool;

    protected float currentDamage;

    protected virtual void Start()
    {
        currentDamage = maxDamage;
    }

    public void Fix(float scale)
    {
        currentDamage = Mathf.Max(0, currentDamage - scale * fixPerSecond * Time.deltaTime);

        if (currentDamage <= 0.0f)
        {
            Complete();
        }
    }

    protected virtual void Complete()
    {
        Destroy(gameObject);
    }
}
