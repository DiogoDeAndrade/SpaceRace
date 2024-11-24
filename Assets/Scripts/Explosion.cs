using UnityEngine;

public class Explosion : DealDamageAOE
{
    [SerializeField] private float      eventProbability = 1.0f;
    [SerializeField] private Hypertag   breachTag;
    [SerializeField] private GameObject breachPrefab;
    [SerializeField] private Hypertag   fireTag;
    [SerializeField] private GameObject firePrefab;

    private void OnDestroy()
    {
        if (Random.Range(0.0f, 1.0f) < eventProbability)
        {
            // Find all objects close enough
            var breachObjects = gameObject.FindObjectsOfTypeWithHypertag<Transform>(breachTag);
            var fireObject = gameObject.FindObjectsOfTypeWithHypertag<Transform>(fireTag);

            float       minDist = float.MaxValue;
            GameObject  prefab = null;
            Transform   targetTransform = null;
            foreach (var t in breachObjects)
            {
                float d = Vector3.Distance(t.position, transform.position);
                if (d < minDist)
                {
                    targetTransform = t;
                    minDist = d;
                    prefab = breachPrefab;
                }
            }
            foreach (var t in fireObject)
            {
                float d = Vector3.Distance(t.position, transform.position);
                if (d < minDist)
                {
                    targetTransform = t;
                    minDist = d;
                    prefab = firePrefab;
                }
            }
            if ((targetTransform) && (prefab))
            {
                Instantiate(prefab, targetTransform.position, targetTransform.rotation);
            }
        }
    }
}
