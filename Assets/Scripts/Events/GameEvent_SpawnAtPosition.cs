using System.Collections.Generic;
using UnityEngine;

public class GameEvent_SpawnAtPosition : GameEvent
{
    [SerializeField] private Hypertag   locationTag;
    [SerializeField] private int        instanceCount = 1;
    [SerializeField] private GameObject prefab;

    List<GameObject> instanceList = new();

    public override bool Init()
    {
        var targets = gameObject.FindObjectsOfTypeWithHypertag<Transform>(locationTag);
        if (targets.Count == 0) return false;

        for (int i = 0; i < Mathf.Min(targets.Count, instanceCount); i++)
        {
            var target = targets.Random(false);

            instanceList.Add(Instantiate(prefab, target.position, target.rotation));
        }

        return true;
    }

    void Update()
    {
        instanceList.RemoveAll((f) => f == null);
        if (instanceList.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}
