using UnityEngine;
using System.Collections.Generic;

public class Engine : ToolContainer
{
    [SerializeField] SpriteRenderer reactorSprite;

    struct FuelData
    {
        public Sprite sprite;
        public float  ammount;
        public float  energy;
        public Color  color;
    }
    List<FuelData> fuelQueue = new();
    float maxFuelAmmount = 0;
    float fuelAmmount = 0;
    float fuelEnergy = 0;
    Material material;

    protected override void Start()
    {
        base.Start();

        reactorSprite.enabled = false;
        material = reactorSprite.material;
    }

    protected override void Update()
    {
        base.Update();

        if (reactorSprite.enabled)
        {
            fuelAmmount -= Time.deltaTime;

            float p = Mathf.Clamp01(fuelAmmount / maxFuelAmmount);
            float c = Mathf.Max(0, p * 4.0f + 0.5f * Mathf.Sin(Time.time * 5.0f));

            material.SetColor("_EmissiveColor", new Color(c, c, c, 1.0f));

            if (fuelAmmount < 0)
            {
                reactorSprite.enabled = false;
            }
        }
        else
        {
            if (fuelQueue.Count > 0)
            {
                var fuel = fuelQueue.PopFirst();
                reactorSprite.enabled = true;
                reactorSprite.sprite = fuel.sprite;
                reactorSprite.color = fuel.color;
                fuelAmmount = maxFuelAmmount = fuel.ammount;
                fuelEnergy = fuel.energy;
            }
        }
    }

    public override bool HangTool(Tool tool)
    {
        var fuel = tool.GetComponent<Fuel>();
        if (fuel != null)
        {
            fuelQueue.Add(new FuelData
            {
                sprite = fuel.sprite,
                color = fuel.color,
                ammount = fuel.ammount,
                energy = fuel.energy,
            });
            Destroy(tool.gameObject);
            return true;
        }

        return false;
    }
}
