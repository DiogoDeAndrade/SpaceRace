using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class ReportScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI raceTime;
    [SerializeField, Scene] private string titleScene;
    [SerializeField] private TextMeshProUGUI labelWinner;
    [SerializeField] private TextMeshProUGUI winner;

    List<bool> playerContinue;

    void Start()
    {
        int t = Mathf.FloorToInt(GameManager.Instance.raceTime);
        float mins = t / 60;
        float secs = t % 60;
        raceTime.text = mins.ToString("00") + ":" + secs.ToString("00");

        playerContinue = new();
        for (int i = 0; i < GameManager.Instance.numPlayers; i++) playerContinue.Add(false);
    }

    void Update()
    {
        // Check inputs
        bool allContinue = true;
        bool displayCompleted = true;
        var players = FindObjectsByType<PlayerReport>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            if (!player.isComplete)
            {
                displayCompleted = false;
                if (player.isWinner) winner.text = "Player " + (player.playerId + 1);
            }
            if (!playerContinue[player.playerId])
            {
                allContinue = false;
                if (player.GetInteractControl().IsDown())
                {
                    playerContinue[player.playerId] = true;
                }
            }
        }

        if (displayCompleted)
        {
            labelWinner.enabled = true;
            winner.enabled = true;            
        }

        if (allContinue)
        {
            FullscreenFader.FadeOut(0.5f, Color.black, () =>
            {
                SceneManager.LoadScene(titleScene);
            });
            enabled = false;
        }
    }
}
 