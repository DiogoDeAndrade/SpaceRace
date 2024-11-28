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


        Destroy(gameObject);

        return true;
    }

    void OnToggle(Player player, bool isUp)
    {
        if (isUp)
        {
            player.AddScore(score);

            if (fusebox)
            {
                fusebox.onToggle -= OnToggle;
            }
            Destroy(gameObject);
        }
    }
}
