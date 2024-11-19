using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameEvent_Blackout : GameEvent
{
    ShipLight[] shipLights;
    Light2D     globalLight;

    public override bool Init()
    {
        if (!base.Init()) return false;

        // Find main global light
        var allLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        foreach (var light in allLights)
        {
            if (light.lightType == Light2D.LightType.Global)
            {
                globalLight = light;
                break;
            }
        }

        globalLight.enabled = true;

        shipLights = FindObjectsByType<ShipLight>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var l in shipLights)
        {
            l.TurnOff();
        }

        return true;
    }

    public void OnDestroy()
    {
        globalLight.enabled = false;
        foreach (var l in shipLights)
        {
            l.TurnOn();
        }
    }
}
