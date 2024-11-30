using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent_SpawnAtPosition : GameEvent
{
    [SerializeField] private Hypertag   locationTag;
    [SerializeField] private int        instanceCount = 1;
    [SerializeField] private GameObject prefab;
    [SerializeField, MinMaxSlider(0.0f, 360.0f)] 
    private Vector2    rotation;

    List<GameObject> instanceList = new();

    public override bool Init()
    {
        var targets = gameObject.FindObjectsOfTypeWithHypertag<Transform>(locationTag);
        if (targets.Count == 0) return false;

        for (int i = 0; i < Mathf.Min(targets.Count, instanceCount); i++)
        {
            var target = targets.Random(false);

            var rot = target.rotation * Quaternion.Euler(0, 0, rotation.Random());            

            instanceList.Add(Instantiate(prefab, target.position, rot));
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
