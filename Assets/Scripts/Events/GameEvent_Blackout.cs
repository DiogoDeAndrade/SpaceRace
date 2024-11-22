using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameEvent_Blackout : GameEvent
{
    public override bool Init()
    {
        if (!base.Init()) return false;

        var fusebox = FindFirstObjectByType<Fusebox>();
        fusebox.Trip();

        Destroy(gameObject);

        return true;
    }
}
