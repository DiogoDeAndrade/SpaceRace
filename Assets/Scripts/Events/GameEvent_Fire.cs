using System.Collections.Generic;
using UnityEngine;

public class GameEvent_Fire : GameEvent
{
    [SerializeField] private Hypertag fireEventTag;
    [SerializeField] private int      fireCount = 1;
    [SerializeField] private Fire     firePrefab;

    List<Fire> fireList = new();

    public override bool Init()
    {
        var targets = gameObject.FindObjectsOfTypeWithHypertag<Transform>(fireEventTag);
        if (targets.Count == 0) return false;

        for (int i = 0; i < Mathf.Min(targets.Count, fireCount); i++)
        {
            var target = targets.Random(false);

            fireList.Add(Instantiate(firePrefab, target.position, target.rotation));
        }

        return true;
    }

    void Update()
    {
        fireList.RemoveAll((f) => f == null);
        if (fireList.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}
