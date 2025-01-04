using NUnit;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.Tweens;

public class HazardDisplay : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private Image icon;

    RectTransform rectTransform;
    float         startX, endX;
    float         offsetY;

    void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    public void SetDisplay(float start, float end, float offsetY, Sprite sprite, Color color)
    {
        float avg = (start + end) * 0.5f;

        rectTransform.anchoredPosition = new Vector2(avg, rectTransform.anchoredPosition.y - offsetY);
        rectTransform.sizeDelta = new Vector2((end - start), rectTransform.sizeDelta.y);

        icon.sprite = sprite;
        bar.color = color;

        startX = start;
        endX = end;
        this.offsetY = offsetY;
    }

    public bool IsOccupied(float offset, float start, float end)
    {
        if ((endX < start) || (startX > end)) return false;

        return (offset == offsetY);
    }

    public bool IsOverlap(float x1, float x2)
    {
        if ((endX < x1) || (startX > x2)) return false;

        return true;
    }

    public void Add(float x1, float x2)
    {
        startX = Mathf.Min(x1, startX, x2);
        endX = Mathf.Max(x1, endX, x2);

        float avg = (startX + endX) * 0.5f;

        rectTransform.anchoredPosition = new Vector2(avg, rectTransform.anchoredPosition.y - offsetY);
        rectTransform.sizeDelta = new Vector2((endX - startX), rectTransform.sizeDelta.y);
    }

    public Sprite sprite => icon.sprite;
}
