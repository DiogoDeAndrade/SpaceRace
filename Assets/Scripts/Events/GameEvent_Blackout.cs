using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameEvent_Blackout : GameEvent
{
    [SerializeField] int score = 50;
    
    Fusebox fusebox;

    public override bool Init()
    {
        if (!base.Init()) return false;

        fusebox = FindFirstObjectByType<Fusebox>();
        fusebox.Trip();

        fusebox.onToggle += OnToggle;

        return true;
    }

    void OnToggle(Player player, bool isUp)
    {
        if (isUp)
        {
            player.AddScore(score);

            fusebox.onToggle -= OnToggle;
            if (gameObject)
            {
                Destroy(gameObject);
            }
        }
    }
}
