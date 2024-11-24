using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public interface Force
    {
        public Vector2 GetForce(Vector2 objectPos);
    }

    [SerializeField] private float maxRace = 240.0f;

    [SerializeField] private float maxOxygen = 100.0f;
    [SerializeField] private float recoverPerSecond = 5.0f;

    GameEventTrigger[] eventTrigger;

    private static GameManager instance;
    
    private float oxygen = 100.0f;
    private float completedRace;
    private float raceElapsedTime;
    private List<Force> forces;

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
    }

    private void Update()
    {
        ChangeOxygen(recoverPerSecond * Time.deltaTime);

        raceElapsedTime += Time.deltaTime;
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
