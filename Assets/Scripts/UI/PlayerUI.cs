using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup        toolCanvasGroup;
    [SerializeField] private Image              toolImage;
    [SerializeField] private Image              toolImageMask;
    [SerializeField] private UIImageEffect      portraitImageEffect;
    [SerializeField] private TextMeshProUGUI    scoreText;
    [SerializeField] private RectTransform      healthMeter;
    
    private Player _player;
    public Player player 
    { 
        get { return _player; } 
        set { _player = value; healthSystem = _player.GetComponent<HealthSystem>(); }
    }

    private CanvasGroup     playerUICanvas;
    private SpriteEffect    spriteEffect;
    private HealthSystem    healthSystem;

    void Start()
    {
        playerUICanvas = GetComponent<CanvasGroup>();
        playerUICanvas.alpha = 0.0f;
        toolCanvasGroup.alpha = 0.0f;
        scoreText.text = "000000";
    }

    void Update()
    {
        if (_player != null)
        {
            if (spriteEffect == null) spriteEffect = _player.GetComponent<SpriteEffect>();
            if (spriteEffect != null)
            {
                var remap = spriteEffect.GetRemap();
                portraitImageEffect.SetRemap(remap);
            }

            playerUICanvas.alpha = Mathf.Clamp01(playerUICanvas.alpha + Time.deltaTime * 4.0f);

            if (_player.hasTool)
            {
                toolCanvasGroup.alpha = Mathf.Clamp01(toolCanvasGroup.alpha + Time.deltaTime * 4.0f);

                toolImage.sprite = _player.toolImage;

                float charge = _player.toolCharge;

                toolImageMask.fillAmount = 1.0f - charge;
            }
            else
            {
                toolCanvasGroup.alpha = Mathf.Clamp01(toolCanvasGroup.alpha - Time.deltaTime * 4.0f);
            }

            scoreText.text = _player.score.ToString("D6");

            float p = healthSystem.normalizedHealth;
            if (_player.isDead) p = 0.0f;
            healthMeter.localScale = new Vector3(p, 1.0f, 1.0f);
        }
        else
        {
            playerUICanvas.alpha = Mathf.Clamp01(playerUICanvas.alpha - Time.deltaTime * 4.0f);
        }
    }
}
