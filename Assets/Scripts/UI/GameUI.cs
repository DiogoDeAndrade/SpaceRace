using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup        oxygenGroup;
    [SerializeField] private Image              oxygenMeter;
    [SerializeField] private RectTransform      rocketImage;
    [SerializeField] private TextMeshProUGUI    clockTimer;
    [SerializeField] private Transform          raceProgressBar;
    [SerializeField] private HazardDisplay      hazardDisplayPrefab; 

    float rocketBarWidth;

    void Start()
    {
        rocketImage.anchoredPosition = Vector2.zero;

        float barWidth = (rocketImage.parent as RectTransform).sizeDelta.x;
        rocketBarWidth = barWidth - rocketImage.sizeDelta.x;

        var hazards = LevelManager.GetDisplayHazards();
        var displays = new List<HazardDisplay>();
        foreach (var hazard in hazards)
        {
            float x1 = hazard.interval.x * barWidth;
            float x2 = hazard.interval.y* barWidth;

            // Check if we can extend
            bool extended = false;
            foreach (var display in displays)
            {
                if ((display.sprite == hazard.sprite) &&
                    (display.IsOverlap(x1, x2)))
                {
                    extended = true;
                    display.Add(x1, x2);
                    break;
                }
            }
            if (extended) continue;

            float offsetY = 0.0f;
            bool  allowed = false;
            while (!allowed)
            {
                allowed = true;
                foreach (var display in displays)
                {
                    if (display.IsOccupied(offsetY, x1, x2))
                    {
                        allowed = false;
                        offsetY += 2.0f;
                        break;
                    }
                }
            }            

            var hDisplay = Instantiate(hazardDisplayPrefab, raceProgressBar);
            hDisplay.SetDisplay(x1, x2, offsetY, hazard.sprite, hazard.color);

            displays.Add(hDisplay);
        }
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
