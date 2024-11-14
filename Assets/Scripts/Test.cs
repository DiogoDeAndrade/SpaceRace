using SciGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public PlayerInput playerInput;

    [InputPlayer(nameof(playerInput))]
    public InputControl testInputControl;

    void Start()
    {
        
    }

    void Update()
    {
    }
}
