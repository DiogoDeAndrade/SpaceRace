using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameEventTrigger[] eventTrigger;

    internal void AddEvent(GameEvent currentEvent)
    {
        throw new NotImplementedException();
    }

    void Start()
    {
        eventTrigger = GetComponents<GameEventTrigger>();
    }
}
