using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class Player : MonoBehaviour
{
    [SerializeField] private int            _playerId = 1;
    [SerializeField] private Transform      grabPoint;
    [SerializeField] private float          grabRadius = 2.0f;
    [SerializeField] private LayerMask      grabMask;
    [SerializeField] private Tooltip        tooltip;

    public int  playerId => _playerId;
    
    private Item grabbedItem;  

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Item canGrab = null;

        var colliders = Physics2D.OverlapCircleAll(grabPoint.position, grabRadius, grabMask);
        foreach (var collider in colliders)
        {
            Item item = collider.GetComponent<Item>();
            if (item != null)
            {
                canGrab = item;
                tooltip.Set(item.transform.position, item.displayName);
                break;
            }
        }
        if (canGrab == null)
        {
            tooltip.Set(tooltip.transform.position, "");
        }
    }

    public void SetPlayerId(int playerId)
    {
        _playerId = playerId;
    }

    private void OnDrawGizmos()
    {
        if (grabPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(grabPoint.position, grabRadius);
        }
    }
}
