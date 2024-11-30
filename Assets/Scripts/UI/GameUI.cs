using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup        oxygenGroup;
    [SerializeField] private Image              oxygenMeter;
    [SerializeField] private RectTransform      rocketImage;
    [SerializeField] private TextMeshProUGUI    clockTimer;

    float rocketBarWidth;

    void Start()
    {
        rocketImage.anchoredPosition = Vector2.zero;

        rocketBarWidth = (rocketImage.parent as RectTransform).sizeDelta.x - rocketImage.sizeDelta.x;
    }

    void Update()
    {
        // Race meter
        float raceProgress = LevelManager.raceProgress;
        rocketImage.anchoredPosition = new Vector2(raceProgress * rocketBarWidth, 0.0f);

        float raceTime = LevelManager.raceTimer;
        int minutes = Mathf.FloorToInt(raceTime / 60);
        int seconds = Mathf.FloorToInt(raceTime) % 60;
        clockTimer.text = $"{minutes.ToString("D2")}:{seconds.ToString("D2")}";

        // Oxygen meter
        if (LevelManager.oxygenPercentage < 1.0f)
        {
            oxygenMeter.fillAmount = LevelManager.oxygenPercentage;
            oxygenGroup.alpha = Mathf.Clamp01(oxygenGroup.alpha + Time.deltaTime * 2.0f);
        }
        else
        {
            oxygenGroup.alpha = Mathf.Clamp01(oxygenGroup.alpha - Time.deltaTime * 2.0f);
        }

        oxygenGroup.gameObject.SetActive(oxygenGroup.alpha > 0.0f);
    }
}
