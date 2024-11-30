using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LaserGun : Tool
{
    [Header("Laser Gun Properties")]
    [SerializeField] Light2D muzzleFlash;
    [SerializeField] Laser   laserPrefab;

    Material material;

    protected override void Start()
    {
        base.Start();

        material = GetComponent<SpriteRenderer>().material;
    }

    protected override void Update()
    {
        base.Update();

        if (currentCharge > 0.0f)
        {
            material.SetColor("_EmissiveColor", Color.white);
        }
        else
        {
            material.SetColor("_EmissiveColor", Color.black);
        }

        if ((_toolActive) && (currentCharge > 0.0f))
        {
            // Shoot
            muzzleFlash.Flash(4.0f, 0.1f, false);

            var laser = Instantiate(laserPrefab, toolPoint.position, toolPoint.rotation);
            laser.owner = owner;
        }
    }
}
