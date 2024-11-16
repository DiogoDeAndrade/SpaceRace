using UnityEngine;

public class GameEvent : MonoBehaviour
{
    protected virtual void Start()
    {
    }

    public virtual bool Init()
    {
        return true;
    }
}
