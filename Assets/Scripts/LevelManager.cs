using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public interface Force
    {
        public Vector2 GetForce(Vector2 objectPos);
    }

    [SerializeField] private float maxRace = 240.0f;

    [SerializeField] private float maxOxygen = 100.0f;
    [SerializeField] private float recoverPerSecond = 5.0f;
    [SerializeField] private CanvasGroup gameOverPanel;
    [SerializeField, Scene] private string titleScene;
    [SerializeField] private CanvasGroup raceOverPanel;
    [SerializeField, Scene] private string raceEndScene;
    [SerializeField] private KeyCode endRaceCheatKey = KeyCode.None;
    [SerializeField] private AudioClip musicClip;

    GameEventTrigger[] eventTrigger;

    private static LevelManager instance;
    
    private float oxygen = 100.0f;
    private float completedRace;
    private float raceElapsedTime;
    private List<Force> forces;
    private bool isGameOver = false;
    private bool isRaceOver = false;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        eventTrigger = GetComponents<GameEventTrigger>();
        oxygen = maxOxygen;
        raceElapsedTime = 0.0f;

        FullscreenFader.FadeIn(0.5f);

        if (musicClip)
        {
            SoundManager.PlayMusic(musicClip);
        }
    }

    private void Update()
    {
        if ((!isGameOver) && (!isRaceOver))
        {
            ChangeOxygen(recoverPerSecond * Time.deltaTime);

            raceElapsedTime += Time.deltaTime;
            GameManager.Instance.raceTime = raceElapsedTime;

            if ((endRaceCheatKey != KeyCode.None) && (Input.GetKeyDown(endRaceCheatKey)))
            {
                completedRace = maxRace;
            }
            if (completedRace >= maxRace)
            {
                raceOverPanel.FadeIn(0.5f);
                isRaceOver = true;
            }
            else
            {
                // Check if players
                var players = FindObjectsByType<Player>(FindObjectsSortMode.None);
                var isAlive = false;
                foreach (var player in players)
                {
                    var hs = player.GetComponent<HealthSystem>();
                    if (hs.isAlive)
                    {
                        isAlive = true;
                        break;
                    }
                }

                if (!isAlive)
                {
                    gameOverPanel.FadeIn(0.5f);
                    isGameOver = true;
                }
            }
        }
        else
        {
            // Check inputs
            var players = FindObjectsByType<Player>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                if (player.GetInteractControl().IsDown())
                {
                    OnContinue();
                }
            }
            
        }
    }

    private void OnContinue()
    {
        // Pressed interact, next screen
        if (isGameOver)
        {
            FullscreenFader.FadeOut(0.5f, Color.black, () =>
            {
                SceneManager.LoadScene(titleScene);
            });
        }
        else
        {
            FullscreenFader.FadeOut(0.5f, Color.black, () =>
            {
                SceneManager.LoadScene(raceEndScene);
            });
        }
    }

    public static void ChangeOxygen(float delta)
    {
        instance.oxygen = Mathf.Clamp(instance.oxygen + delta, 0, instance.maxOxygen);
    }

    public static float oxygenPercentage
    {
        get
        {
            return instance.oxygen / instance.maxOxygen;
        }
    }

    public static void ChangeRace(float delta)
    {
        instance.completedRace = Mathf.Clamp(instance.completedRace + delta, 0, instance.maxRace);
    }

    public static float raceProgress
    {
        get
        {
            return Mathf.Clamp01(instance.completedRace / instance.maxRace);
        }
    }

    public static float raceTimer
    {
        get
        {
            return Mathf.Max(0, instance.raceElapsedTime);
        }
    }

    public static void AddForce(Force force)
    {
        if (instance.forces == null) instance.forces = new();
        instance.forces.Add(force);
    }
    public static void RemoveForce(Force force)
    {
        if (instance.forces == null) return;
        instance.forces.Remove(force);
    }
    public static Vector2 GetForce(Vector3 currentPos)
    {
        if (instance.forces == null) return Vector2.zero;

        instance.forces.RemoveAll((x) => x == null);

        Vector2 ret = Vector2.zero;
        foreach (var force in instance.forces)
        {
            ret = ret + force.GetForce(currentPos);
        }

        return ret;
    }
}
