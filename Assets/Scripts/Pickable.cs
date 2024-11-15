using UnityEngine;

public class Pickable : Item
{
    public override bool canInteract => true;

    public override bool Interact(Player player)
    {
        if (player.hasInventorySpace)
        {
            player.AddToInventory(this);
            gameObject.SetActive(false);
            return true;
        }

        return false;
    }
}
