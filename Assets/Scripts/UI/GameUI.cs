using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup    oxygenGroup;
    [SerializeField] private Image          oxygenMeter;

    void Start()
    {
                
    }

    void Update()
    {
        if (GameManager.oxygenPercentage < 1.0f)
        {
            oxygenMeter.fillAmount = GameManager.oxygenPercentage;
            oxygenGroup.alpha = Mathf.Clamp01(oxygenGroup.alpha + Time.deltaTime * 2.0f);
        }
        else
        {
            oxygenGroup.alpha = Mathf.Clamp01(oxygenGroup.alpha - Time.deltaTime * 2.0f);
        }

        oxygenGroup.gameObject.SetActive(oxygenGroup.alpha > 0.0f);
    }
}
