using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fusebox : Interactable
{
    [SerializeField] private ShipLight[]    shipLights;
    [SerializeField] private Light2D        globalLight;
    [SerializeField] private AudioClip      breakerSnd;
    [SerializeField] private AudioClip      activateSnd;

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
            if (breakerSnd)
            {
                if (!isUp) SoundManager.PlaySound(SoundType.PrimaryFX, breakerSnd);
                else SoundManager.PlaySound(SoundType.PrimaryFX, activateSnd);
            }

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
        if (breakerSnd) SoundManager.PlaySound(SoundType.PrimaryFX, breakerSnd);
        UpdateLight();
    }
}
