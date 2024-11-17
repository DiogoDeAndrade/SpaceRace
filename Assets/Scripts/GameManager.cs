using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float maxOxygen = 100.0f;
    [SerializeField] private float recoverPerSecond = 5.0f;

    GameEventTrigger[] eventTrigger;

    private static GameManager instance;
    private float oxygen = 100.0f;

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
    }

    private void Update()
    {
        ChangeOxygen(recoverPerSecond * Time.deltaTime);
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
}
