using UnityEngine;

public class Tool : Item
{

    public override bool canInteract => true;
    public override bool Interact(Player player)
    {
        if (player.hasTool)
        {
            player.DropTool();
        }

        player.SetTool(this);

        SetPhysics(false);

        return true;
    }

    public void Throw(Vector2 direction)
    {
        transform.SetParent(null);
        owner = null;

        SetPhysics(true);

        rb.linearVelocity = direction * 100.0f + Vector2.up * 75.0f;
    }
}
