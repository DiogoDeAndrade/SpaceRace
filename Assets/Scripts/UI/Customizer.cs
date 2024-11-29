using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Customizer : UIGroup
{
    [SerializeField] int                        playerId;
    [SerializeField] UIImageEffect              uiEffect;
    [SerializeField] ColorPalette               originalPalette;
    [SerializeField] UIDiscreteColorSelector    hairColorSelector;
    [SerializeField] UIDiscreteColorSelector    bodyColorSelector;
    [SerializeField] UIButton                   continueButton;

    ColorPalette palette;

    protected override void Start()
    {
        base.Start();

        palette = originalPalette.Clone();

        hairColorSelector.onChange += OnColorChange;
        bodyColorSelector.onChange += OnColorChange;
        continueButton.onInteract += OnContinue;

        OnColorChange(null);

        if (GameManager.Instance.numPlayers <= playerId)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnColorChange(BaseUIControl control)
    {
        palette = CharacterCustomization.BuildPalette(originalPalette, hairColorSelector.value, bodyColorSelector.value);

        uiEffect.SetRemap(palette);
    }

    void OnContinue(BaseUIControl control)
    {
        selectedControl = null;
        SetUI(false);
        continueButton.gameObject.SetActive(false);

        PlayerInput playerInput = GetComponent<PlayerInput>();
        var devices = playerInput.devices[0];        

        var pd = new GameManager.PlayerData
        {
            hairColor = hairColorSelector.value,
            bodyColor = bodyColorSelector.value,
            deviceId = devices.deviceId,
        };

        GameManager.Instance.SetPlayerData(playerId, pd);
    }
}
