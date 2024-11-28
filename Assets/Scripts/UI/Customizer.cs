using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Customizer : MonoBehaviour
{
    [SerializeField] private float       moveCooldown = 0.1f;
    [SerializeField] private PlayerInput playerInput;

    [SerializeField, InputPlayer(nameof(playerInput))]
    InputControl horizontalControl;
    [SerializeField, InputPlayer(nameof(playerInput))]
    InputControl verticalControl;


    [SerializeField] UIControl initialControl;

    float cooldownTimer;

    void Start()
    {
        horizontalControl.playerInput = playerInput;
        verticalControl.playerInput = playerInput;

        if (initialControl)
        {
            initialControl.Select();
        }
    }

    void Update()
    {
        if (UIControl.currentlySelected)
        {
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0.0f)
            {
                if (verticalControl.GetAxis() < 0.0f)
                {
                    UIControl.currentlySelected.navDown?.Select();
                    cooldownTimer = moveCooldown;
                }
                else if (verticalControl.GetAxis() > 0.0f)
                {
                    UIControl.currentlySelected.navUp?.Select();
                    cooldownTimer = moveCooldown;
                }
            }
        }
    }
}
