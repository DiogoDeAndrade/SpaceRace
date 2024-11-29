using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField, Scene] private string gameScene;

    Customizer[] customizers;

    void Start()
    {
        customizers = FindObjectsByType<Customizer>(FindObjectsSortMode.None);

        FullscreenFader.FadeIn(0.5f);
    }

    void Update()
    {
        bool noActive = true;
        foreach (var customizer in customizers)
        {
            if (customizer.uiEnable)
            {
                noActive = false;
                break;
            }
        }

        if (noActive)
        {
            FullscreenFader.FadeOut(0.5f, Color.black, () =>
            {
                SceneManager.LoadScene(gameScene);
            });
            enabled = false;
        }
    }
}
