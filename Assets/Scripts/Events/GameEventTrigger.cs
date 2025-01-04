using NaughtyAttributes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class GameEventTrigger : MonoBehaviour
{
    public enum CooldownType { None, SpecificEvent, AnyEvent };
    public enum TriggerType { Time, RaceInterval };

    [SerializeField]
    private Hypertag        eventTag;
    [SerializeField]
    private CooldownType    cooldownType;
    [SerializeField, HideIf(nameof(cooldownType), CooldownType.None)]
    private float           cooldownTime;
    [SerializeField, ShowIf(nameof(cooldownType), CooldownType.SpecificEvent)]
    private Hypertag        cooldownEventTag;
    [SerializeField] 
    private float           playerCountMultiplier = 1.0f;
    [SerializeField] 
    private TriggerType     type;
    [SerializeField] 
    private bool            retrigger = true;
    [SerializeField, MinMaxSlider(1.0f, 240.0f), ShowIf(nameof(type), TriggerType.Time)] 
    private Vector2         initialInterval = new Vector2(10.0f, 10.0f);
    [SerializeField, MinMaxSlider(1.0f, 240.0f), ShowIf(nameof(type), TriggerType.Time)] 
    private Vector2         repeatInterval = new Vector2(10.0f, 10.0f);
    [SerializeField, MinMaxSlider(0.0f, 1.0f), ShowIf(nameof(type), TriggerType.RaceInterval)] 
    private Vector2         _interval = new Vector2(0.0f, 1.0f);
    [SerializeField, ShowIf(nameof(type), TriggerType.RaceInterval), Label("Probability per second (%)")]
    private float           probabilityPerSecond = 5;
    [SerializeField]
    private GameEvent       eventPrefab;
    [SerializeField, Header("Display")]
    private Sprite          _sprite;
    [SerializeField]
    private Color          _color = Color.white;
    [SerializeField, Header("Debug")]
    private KeyCode         cheatKey = KeyCode.None;

    private float               timer = 0.0f;
    private GameEvent           currentEvent;
    private int                 triggerCount = 0;
    private float               accumTimer;

    struct PreviousEvent
    {
        public GameEventTrigger evt;
        public float            time;
    }
    static List<PreviousEvent> previousEvents = new();

    void Start()
    {
        timer = initialInterval.Random();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentEvent == null)
        {
            bool canRun = true;
            var nPlayers = GameManager.Instance.numPlayers;

            if (cheatKey != KeyCode.None)
            {
                if (Input.GetKeyDown(cheatKey))
                {
                    TriggerEvent();
                    return;
                }
            }

            float cd = cooldownTime / (playerCountMultiplier * nPlayers);

            switch (cooldownType)
            {
                case CooldownType.None:
                    break;
                case CooldownType.SpecificEvent:
                    for (int i = previousEvents.Count - 1; i >= 0; i--)
                    {
                        if (previousEvents[i].evt.eventTag == cooldownEventTag)
                        {
                            if (Time.time - previousEvents[i].time < cd)
                            {
                                canRun = false;
                                break;
                            }
                        }
                    }
                    break;
                case CooldownType.AnyEvent:
                    for (int i = previousEvents.Count - 1; i >= 0; i--)
                    {
                        if (Time.time - previousEvents[i].time < cd)
                        {
                            canRun = false;
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (canRun)
            {
                float deltaTime = Time.deltaTime;
                if (playerCountMultiplier > 0.0f)
                {
                    deltaTime = playerCountMultiplier * nPlayers * Time.deltaTime;
                }
                switch (type)
                {
                    case TriggerType.Time:
                        timer -= deltaTime;
                        if (timer < 0.0f)
                        {
                            if (TriggerEvent())
                            {
                                timer = repeatInterval.Random();
                            }
                        }
                        break;
                    case TriggerType.RaceInterval:
                        {
                            float p = LevelManager.raceProgress;
                            if ((p >= _interval.x) && (p <= _interval.y))
                            {
                                accumTimer += deltaTime;
                                while (accumTimer > 1.0f)
                                {
                                    p = Random.Range(0.0f, 100.0f);
                                    if (p < probabilityPerSecond)
                                    {
                                        TriggerEvent();
                                    }

                                    accumTimer -= 1.0f;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    [Button("Trigger Now")]
    bool TriggerEvent()
    {
        triggerCount++;

        if (eventPrefab)
        {
            currentEvent = Instantiate(eventPrefab, transform);
            if (!currentEvent.Init()) return false;
        }
        if (!retrigger)
        {
            enabled = false;
        }
        previousEvents.Add(new PreviousEvent
        {
            evt = this,
            time = Time.time,
        });

        return true;
    }

    public bool canDisplay => (enabled) && (type == TriggerType.RaceInterval) && (_sprite != null);
    public Sprite sprite => _sprite;
    public Color color => _color;
    public Vector2 interval => _interval;
}
