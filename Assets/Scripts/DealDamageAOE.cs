using System.Collections;
using UnityEngine;

public class DealDamageAOE : MonoBehaviour
{
    [SerializeField] private float damageDelay = 0.0f;
    [SerializeField] private float damageRadius = 50.0f;
    [SerializeField] private float damage = 0.0f;

    void Start()
    {
        StartCoroutine(DealDamageCR());            
    }

    IEnumerator DealDamageCR()
    {
        if (damageDelay > 0.0f)
        {
            yield return new WaitForSeconds(damageDelay);

            // Find all damage receivers
            var healthSystems = FindObjectsByType<HealthSystem>(FindObjectsSortMode.None);
            foreach (var healthSystem in healthSystems)
            {
                float distance = Vector3.Distance(transform.position, healthSystem.transform.position);
                if (distance < damageRadius)
                {
                    healthSystem.DealDamage(damage, transform.position);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
