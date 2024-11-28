using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameEvent_Pipes : GameEvent
{
    public override bool Init()
    {
        if (!base.Init()) return false;

        var pipes = FindObjectsByType<Pipe>(FindObjectsSortMode.None);

        var pipe = pipes[Random.Range(0, pipes.Length)];
        pipe.Break();

        Destroy(gameObject);

        return true;
    }
}
