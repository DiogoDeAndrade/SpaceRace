using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private ParticleSystem     starsPS;
    [SerializeField] private List<GameObject>   prefabs;
    [SerializeField] private List<Transform>    spawnPoints;
    [SerializeField] private int                maxElements = 1;
    [SerializeField] private Vector2            timeInterval = new Vector2(20.0f, 60.0f);
    [SerializeField] private float              horizontalSpeed = 100;
    [SerializeField] private Vector2            bounds = new Vector2(-320.0f, 320.0f);
    [SerializeField] private Vector2            scaleModifier = new Vector2(0.5f, 1.5f);
    [SerializeField, Range(0.1f, 4.0f)] 
    private float speedScale = 1.0f;

    struct BackgroundElem
    {
        public float        s;
        public float        extentsX;
        public GameObject   element;
    };

    List<BackgroundElem>    activeObjects;
    float                   timer;

    public float GetSpeedScale() => speedScale;
    public void SetSpeedScale(float speedScale) { this.speedScale = speedScale; }

    void Start()
    {
        activeObjects = new();
        SpawnElement();
        timer = timeInterval.Random();
    }

    void Update()
    {
        activeObjects.RemoveAll((e) => e.element == null);

        if (timer > 0)
        {
            timer -= Time.deltaTime * speedScale;
            if (timer <= 0.0f)
            {
                SpawnElement();
                timer = timeInterval.Random();
            }
        }

        foreach (var obj in activeObjects)
        {
            obj.element.transform.position = obj.element.transform.position - horizontalSpeed * obj.s * Time.deltaTime * speedScale * obj.element.transform.right;

            if (obj.element.transform.position.x + obj.extentsX < bounds.x)
            {
                Destroy(obj.element);
            }
        }

        if (starsPS)
        {
            var main = starsPS.main;
            main.simulationSpeed = speedScale;
        }
    }

    void SpawnElement()
    {
        if (activeObjects.Count >= maxElements) return;

        var spawnPoint = spawnPoints.Random();
        var prefab = prefabs.Random();

        float s = scaleModifier.Random();

        var newObject = Instantiate(prefab, transform);
        newObject.transform.position = spawnPoint.position;
        newObject.transform.rotation = spawnPoint.rotation;
        newObject.transform.localScale = Vector3.one * s;

        float extentsX = 0.0f;
        var spriteRenderer = newObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer) extentsX = spriteRenderer.bounds.extents.x;
        var asteroidField = newObject.GetComponent<AsteroidField>();
        if (asteroidField != null)
        {
            extentsX = asteroidField.GetWidth() * 0.5f;
            newObject.transform.localScale = Vector3.one;
        }

        var tmp = new BackgroundElem
        {
            s = s,
            extentsX = extentsX,
            element = newObject,
        };

        // Update to account for width
        newObject.transform.position = new Vector3(bounds.y + extentsX, spawnPoint.position.y, spawnPoint.position.z - s);

        activeObjects.Add(tmp);
    }
}
