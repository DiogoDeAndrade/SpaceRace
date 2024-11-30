using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReport : MonoBehaviour
{
    [SerializeField] private int                _playerId = 0;
    [SerializeField] private ColorPalette       originalPalette;
    [SerializeField] private UIImageEffect      playerPortrait;
    [SerializeField] private float              timeToStart = 2.0f;
    [SerializeField] private float              scoreGrowTime = 5.0f;
    [SerializeField] private AnimationCurve     animationCurve;
    [SerializeField] private TextMeshProUGUI    scoreText;
    [SerializeField] private RectTransform      scoreBar;
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton]
    private InputControl interactControl;
    [SerializeField] private ParticleSystem fireworksPS;

    float maxScore;
    float       thisScore;
    float       currentScore;
    CanvasGroup canvasGroup;

    public int playerId => _playerId;
    public bool isComplete => (thisScore == currentScore);
    public bool isWinner => (thisScore == maxScore);

    void Start()
    {
        if (playerId >= GameManager.Instance.numPlayers)
        {
            Destroy(gameObject);
            return;
        }

        var pd = GameManager.Instance.GetPlayerData(playerId);
        var modifiedPalette = CharacterCustomization.BuildPalette(originalPalette, pd.hairColor, pd.bodyColor);

        playerPortrait.SetRemap(modifiedPalette);

        interactControl.playerInput = playerInput;

        InputDevice inputDevice = null;
        if (pd.deviceId != -1)
        {
            foreach (var device in InputSystem.devices)
            {
                if (device.deviceId == pd.deviceId)
                {
                    inputDevice = device;
                    break;
                }
            }
            if (inputDevice != null)
            {
                StartCoroutine(SwitchCurrentControlSchemeCR(inputDevice));
            }
        }
        playerInput.enabled = true;

        maxScore = 0.0f;
        for (int i = 0; i < GameManager.Instance.numPlayers; i++)
        {
            maxScore = Mathf.Max(maxScore, GameManager.Instance.GetPlayerData(i).score);
        }

        thisScore = pd.score;

        int keys = 4;
        float keyInc = 1.0f / (float)keys;
        animationCurve = new();
        animationCurve.AddKey(0, 0);
        for (int i = 1; i < keys - 1; i++)
        {
            animationCurve.AddKey(i * keyInc, Mathf.Clamp01(i * keyInc + Random.Range(-keyInc * 0.4f, keyInc * 0.4f)));
        }
        animationCurve.AddKey(1, 1);

        canvasGroup = GetComponent<CanvasGroup>();

        StartCoroutine(GrowScoreCR());
    }

    private IEnumerator SwitchCurrentControlSchemeCR(InputDevice inputDevice)
    {
        yield return null;
        yield return null;

        string scheme = playerInput.currentControlScheme;
        if (string.IsNullOrEmpty(scheme)) scheme = "Gamepad";

        playerInput.SwitchCurrentControlScheme(scheme, inputDevice);
    }

    IEnumerator GrowScoreCR()
    {
        yield return new WaitForSeconds(timeToStart);

        float t = 0.0f;
        float tInc = 1.0f / scoreGrowTime;

        while (t <= 1.0f)
        {
            float tt = animationCurve.Evaluate(t);
            t += tInc * Time.deltaTime;

            currentScore = Mathf.FloorToInt(tt * thisScore);

            UpdateScoreDisplay();

            yield return null;
        }

        currentScore = thisScore;
        UpdateScoreDisplay();

        if (isWinner)
        {
            fireworksPS.Play();
        }
        else
        {
            canvasGroup.FadeTo(0.5f, 0.5f);
        }
    }

    void UpdateScoreDisplay()
    {
        scoreText.text = $"Score: {currentScore.ToString("000000")}";
        scoreBar.sizeDelta = new Vector2(scoreBar.sizeDelta.x, 150.0f * currentScore / maxScore);
    }

    public InputControl GetInteractControl() => interactControl;

}
