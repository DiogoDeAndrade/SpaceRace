using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using NaughtyAttributes;

public class AsteroidField : MonoBehaviour
{
    [SerializeField] private int            asteroidCount = 40;
    [SerializeField] private Vector2        relativeSpeed = new Vector2(0.5f, 1.5f);
    [SerializeField] private Rect           spawnBounds;
    [SerializeField,SortingLayer] 
    private int            layerId;
    [SerializeField] private List<Sprite>   sprites;

    struct Asteroid
    {
        public Transform asteroid;
        public float     speed;
    }

    Background background;

    private List<Asteroid> asteroids;

    public float GetWidth() => spawnBounds.width;

    void Start()
    {
        Vector3 extents = new Vector2(0.5f * spawnBounds.width, 0.5f * spawnBounds.height);

        asteroids = new();
        for (int i = 0; i < asteroidCount; i++)
        {
            float speed = relativeSpeed.Random();

            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(Random.Range(-extents.x, extents.x), Random.Range(-extents.y, extents.y), -speed);
            go.transform.localScale = new Vector3(speed, speed, speed);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerID = layerId;
            sr.sprite = sprites.Random();

            asteroids.Add(new Asteroid
            {
                asteroid = go.transform,
                speed = speed
            });
        }

        background = GetComponentInParent<Background>();
    }

    void Update()
    {
        float speedModifier = (background) ? (background.GetSpeedScale()) : (1.0f);
        foreach (var asteroid in asteroids)
        {
            asteroid.asteroid.localPosition -= Vector3.right * (asteroid.speed * 5.0f) * speedModifier * Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 extents = new Vector2(0.5f * spawnBounds.width, 0.5f * spawnBounds.height);
        Vector3 p1 = spawnBounds.min - Vector2.right * extents.x - Vector2.up * extents.y;
        Vector3 p2 = spawnBounds.min - Vector2.right * extents.x + Vector2.up * extents.y;
        Vector3 p3 = spawnBounds.min + Vector2.right * extents.x - Vector2.up * extents.y;
        Vector3 p4 = spawnBounds.min + Vector2.right * extents.x + Vector2.up * extents.y;

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p1, p3);
        Gizmos.DrawLine(p2, p4);
        Gizmos.DrawLine(p3, p4);
    }
}
