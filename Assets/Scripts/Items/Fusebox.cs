using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fusebox : Interactable
{
    [SerializeField] private ShipLight[]    shipLights;
    [SerializeField] private Light2D        globalLight;

    private bool isOpen = false;
    private bool isUp = true;

    public delegate void OnToggle(Player player, bool isUp);
    public event OnToggle onToggle;

    void Update()
    {
        anim.SetBool("Open", isOpen);
        anim.SetBool("Up", isUp);
    }

    public override void Interact(Player player)
    {
        if (isOpen)
        {
            isUp = !isUp;
            UpdateLight();

            onToggle?.Invoke(player, isUp);
        }
        else
        {
            isOpen = true;
        }
    }

    void UpdateLight()
    {
        globalLight.enabled = !isUp;

        foreach (var light in shipLights)
        {
            if (isUp) light.TurnOn();
            else light.TurnOff();
        }
    }

    public void Trip()
    {
        isUp = false;
        UpdateLight();
    }
}
