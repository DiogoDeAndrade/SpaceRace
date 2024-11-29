using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Title : UIGroup
{
    [SerializeField] UIButton onePlayerButton;
    [SerializeField] UIButton twoPlayerButton;
    [SerializeField] UIButton creditsButton;
    [SerializeField] UIButton quitButton;
    [SerializeField, Scene] string characterSelectScene;
    [SerializeField] BigTextScroll creditsScroll;

    protected override void Start()
    {
        base.Start();

        onePlayerButton.onInteract += StartOnePlayer;
        twoPlayerButton.onInteract += StartTwoPlayers;
        creditsButton.onInteract += ShowCredits;
        quitButton.onInteract += QuitGame;
    }

    private void ShowCredits(BaseUIControl control)
    {
        _uiEnable = false;
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.FadeOut(0.5f);

        canvasGroup = creditsScroll.GetComponent<CanvasGroup>();
        canvasGroup.FadeIn(0.5f);

        creditsScroll.Reset();

        creditsScroll.onEndScroll += BackToOptions;
    }

    private void BackToOptions()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.FadeIn(0.5f);

        canvasGroup = creditsScroll.GetComponent<CanvasGroup>();
        canvasGroup.FadeOut(0.5f);

        _uiEnable = true;
        selectedControl = onePlayerButton;

        creditsScroll.onEndScroll -= BackToOptions;
    }

    private void StartOnePlayer(BaseUIControl control)
    {
        StartGame(1);
    }
    private void StartTwoPlayers(BaseUIControl control)
    {
        StartGame(2);
    }

    void StartGame(int nPlayers)
    {
        _uiEnable = false;
        GameManager.Instance.numPlayers = nPlayers;
        FullscreenFader.FadeOut(0.5f, Color.black, () =>
        {
            SceneManager.LoadScene(characterSelectScene);
        });

    }

    private void QuitGame(BaseUIControl control)
    {
        _uiEnable = false;
        FullscreenFader.FadeOut(0.5f, Color.black, () =>
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
#else
            Application.Quit();
#endif
        });
    }

}
