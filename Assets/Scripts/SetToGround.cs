using UnityEngine;

public class SetToGround : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float     yOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, 40, groundMask);
        if (hit.collider != null)
        {
            transform.position = hit.point + Vector2.up * yOffset;
        }
    }
}
