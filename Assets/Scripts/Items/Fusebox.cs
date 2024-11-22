using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fusebox : Interactable
{
    [SerializeField] private ShipLight[]    shipLights;
    [SerializeField] private Light2D        globalLight;

    private bool isOpen = false;
    private bool isUp = true;

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
