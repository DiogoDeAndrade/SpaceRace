using UnityEngine;

public class Medikit : Tool
{
    [SerializeField] private float healthGain;

    protected override bool RunTool(Collider2D collider)
    {
        Player otherPlayer = collider.GetComponent<Player>();
        if ((otherPlayer != null) && (otherPlayer != owner))
        {
            var hs = otherPlayer.GetComponent<HealthSystem>();
            if (hs.normalizedHealth < 1.0f)
            {
                hs.Heal(healthGain, true);
                return true;
            }
        }

        return false;
    }
}
