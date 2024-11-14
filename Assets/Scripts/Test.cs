using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public PlayerInput playerInput;

    [InputPlayer(nameof(playerInput))]
    public InputControl testInputControl;

    void Start()
    {
        testInputControl.playerInput = playerInput;
    }

    void Update()
    {
        float v = testInputControl.GetAxis();
        if (Mathf.Abs(v) > 1e-3)
        {
            Debug.Log($"{name}. testInputControl = {v}");
        }
    }
}
