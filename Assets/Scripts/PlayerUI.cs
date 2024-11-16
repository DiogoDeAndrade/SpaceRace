using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup    toolCanvasGroup;
    [SerializeField] private Image          toolImage;
    [SerializeField] private Image          toolImageMask;
    [SerializeField] private UIImageEffect  portraitImageEffect;
    public Player player { get; set; }

    private CanvasGroup     playerUICanvas;
    private SpriteEffect    spriteEffect;

    void Start()
    {
        playerUICanvas = GetComponent<CanvasGroup>();
        playerUICanvas.alpha = 0.0f;
        toolCanvasGroup.alpha = 0.0f;
    }

    void Update()
    {
        if (player != null)
        {
            if (spriteEffect == null) spriteEffect = player.GetComponent<SpriteEffect>();
            if (spriteEffect != null)
            {
                var remap = spriteEffect.GetRemap();
                portraitImageEffect.SetRemap(remap);
            }

            playerUICanvas.alpha = Mathf.Clamp01(playerUICanvas.alpha + Time.deltaTime * 4.0f);

            if (player.hasTool)
            {
                toolCanvasGroup.alpha = Mathf.Clamp01(toolCanvasGroup.alpha + Time.deltaTime * 4.0f);

                toolImage.sprite = player.toolImage;

                float charge = player.toolCharge;

                toolImageMask.fillAmount = 1.0f - charge;
            }
            else
            {
                toolCanvasGroup.alpha = Mathf.Clamp01(toolCanvasGroup.alpha - Time.deltaTime * 4.0f);
            }
        }
        else
        {
            playerUICanvas.alpha = Mathf.Clamp01(playerUICanvas.alpha - Time.deltaTime * 4.0f);
        }
    }
}
