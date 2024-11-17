using System;
using Unity.VisualScripting;
using UnityEngine;

public class Accident : MonoBehaviour
{
    [SerializeField] protected float    maxDamage = 100.0f;
    [SerializeField] protected float    fixPerSecond = 25.0f;
    [SerializeField] protected float    damagePerSecond = 0.0f;
    [SerializeField] protected ToolDef  _fixTool;
    [SerializeField] protected int      score = 100;

    public ToolDef fixTool => _fixTool;

    protected float currentDamage;

    protected virtual void Start()
    {
        currentDamage = maxDamage;
    }

    public void Fix(Player player, float scale)
    {
        if (currentDamage > 0)
        {
            currentDamage = Mathf.Max(0, currentDamage - scale * fixPerSecond * Time.deltaTime);

            if (currentDamage <= 0.0f)
            {
                Complete(player);
            }
        }
    }

    protected virtual void Update()
    {
        if (currentDamage > 0.0f)
        {
            currentDamage = Mathf.Clamp(currentDamage + damagePerSecond * Time.deltaTime, 0, maxDamage);
        }
    }

    protected virtual void Complete(Player player)
    {
        player.AddScore(score);
    }
}
