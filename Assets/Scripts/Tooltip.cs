using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    TextMeshProUGUI text;
    CanvasGroup     canvasGroup;

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;   
    }

    public void Set(Vector3 pos, string text)
    {
        if (text == "")
        {
            canvasGroup.FadeOut(0.1f);
        }
        else
        {
            this.text.text = text;
            canvasGroup.FadeIn(0.1f);
        }
    }
}
