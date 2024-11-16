using NaughtyAttributes;
using System.Diagnostics.Tracing;
using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    public enum TriggerType { Time };

    [SerializeField] 
    private TriggerType     type;
    [SerializeField] 
    private bool            retrigger = true;
    [SerializeField, MinMaxSlider(1.0f, 240.0f), ShowIf("type", TriggerType.Time)] 
    private Vector2         initialInterval = new Vector2(10.0f, 10.0f);
    [SerializeField, MinMaxSlider(1.0f, 240.0f), ShowIf("type", TriggerType.Time)] 
    private Vector2         repeatInterval = new Vector2(10.0f, 10.0f);
    [SerializeField]
    private GameEvent       eventPrefab;
    [SerializeField]
    private KeyCode         cheatKey = KeyCode.None;

    private float               timer = 0.0f;
    private GameEvent           currentEvent;
    private int                 triggerCount = 0;

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

            if (cheatKey != KeyCode.None)
            {
                if (Input.GetKeyDown(cheatKey))
                {
                    TriggerEvent();
                    return;
                }
            }

            if (canRun)
            {
                switch (type)
                {
                    case TriggerType.Time:
                        timer -= Time.deltaTime;
                        if (timer < 0.0f)
                        {
                            if (TriggerEvent())
                            {
                                timer = repeatInterval.Random();
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
            Destroy(this);
        }

        return true;
    }
}
