using UnityEngine;

public class RocketMove : MonoBehaviour
{
    [SerializeField] Vector2 bounds = new Vector2(-1200.0f, 1200.0f);
    [SerializeField] float   speed = 100.0f;

    RectTransform rectTransform;

    void Start()
    {
        rectTransform = transform as RectTransform;
        rectTransform.anchoredPosition = new Vector2(bounds.x, rectTransform.anchoredPosition.y);
    }

    void Update()
    {
        rectTransform.anchoredPosition = rectTransform.anchoredPosition + Vector2.right * speed * Time.deltaTime;
        if (rectTransform.anchoredPosition.x > bounds.y)
        {
            rectTransform.anchoredPosition = new Vector2(bounds.x, rectTransform.anchoredPosition.y);
        }
    }
}
