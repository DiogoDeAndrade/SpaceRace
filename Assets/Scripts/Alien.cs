using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Alien : MonoBehaviour
{
    [SerializeField] private float          initialDelay = 5.0f;
    [SerializeField] private float          moveSpeed = 100.0f;
    [SerializeField] private float          attackRange = 40.0f;
    [SerializeField] private Hypertag       ventTag;
    [SerializeField] private Transform      attackPos;
    [SerializeField] private float          attackRadius;
    [SerializeField] private float          attackDamage;
    [SerializeField] private LayerMask      groundMask;
    [SerializeField] private ParticleSystem deathPS;

    bool canMove = false;

    SpriteRenderer  spriteRenderer;
    Transform       currentVent;
    Transform       nextVent;
    Rigidbody2D     rb;
    Animator        animator;
    Collider2D      mainCollider;
    SpriteEffect    spriteEffect;
    bool            dead;

    public bool isVulnerable => canMove;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCollider = GetComponent<Collider2D>();
        spriteEffect = GetComponent<SpriteEffect>();

        SelectTargetVent();
        SnapToFloor();

        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        StartCoroutine(WaitAndStartCR());
    }

    IEnumerator WaitAndStartCR()
    {
        canMove = false;

        // Do sound to warn an alien is coming and shake the vent
        currentVent.Shake2d(1.0f, 1.0f);
        yield return new WaitForSeconds(initialDelay);

        spriteRenderer.FadeTo(Color.white, 1.0f, "SpriteFade");

        yield return new WaitForSeconds(1.0f);

        canMove = true;
    }

    IEnumerator FadeAndMoveCR()
    {
        canMove = false;

        spriteRenderer.FadeTo(Color.white.ChangeAlpha(0.0f), 1.0f, "SpriteFade");

        yield return new WaitForSeconds(1.0f);

        var allVents = gameObject.FindObjectsOfTypeWithHypertag<Transform>(ventTag);
        var newVent = allVents.Random();
        transform.position = newVent.position;

        SelectTargetVent();
        SnapToFloor();

        StartCoroutine(WaitAndStartCR());
    }

    void Update()
    {
        if (dead) return;

        Transform runToTarget = null;
        bool targetIsVent = false;

        // Check if a player is nearby and turn to him
        var players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        if (players.Length > 0)
        {
            foreach (var player in players)
            {
                if (player.isDead) continue;
                if (Mathf.Abs(player.transform.position.y - transform.position.y) > 40.0f) continue;

                float dist = players[0].transform.position.x - transform.position.x;
                if (Mathf.Abs(dist) < 150.0f)
                {
                    runToTarget = player.transform;

                    if ((Mathf.Abs(dist) < attackRange) && (canMove))
                    {
                        TurnTo(Mathf.Sign(dist));
                        canMove = false;
                        animator.SetTrigger("Attack");
                    }

                    break;
                }
                else if (((dist * transform.right.x) > 0.0f) && ((dist * transform.right.x) < 300))
                {
                    runToTarget = player.transform;
                    break;
                }
            }            
        }

        if (runToTarget == null)
        {
            runToTarget = nextVent.transform;
            targetIsVent = true;
        }

        if (runToTarget != null)
        { 
            // Turn to target
            float dist = runToTarget.position.x - transform.position.x;
            float s = Mathf.Sign(dist);
            TurnTo(s);

            if (canMove)
            {
                rb.linearVelocityX = moveSpeed * s;
                
                if ((targetIsVent) && (mainCollider.OverlapPoint(nextVent.transform.position)))
                {
                    StartCoroutine(FadeAndMoveCR());
                }
            }
            else
            {
                rb.linearVelocityX = 0.0f;
            }
        }

        animator.SetFloat("AbsVelocityX", Mathf.Abs(rb.linearVelocityX));
    }

    void TurnTo(float s)
    {
        if ((s < 0) && (transform.right.x > 0)) transform.rotation = Quaternion.Euler(0, 180, 0);
        else if ((s > 0) && (transform.right.x < 0)) transform.rotation = Quaternion.identity;
    }

    void SelectTargetVent()
    {
        var allVents = gameObject.FindObjectsOfTypeWithHypertag<Transform>(ventTag);

        float minDist = float.MaxValue;
        foreach (var t in allVents)
        {
            float d = Vector3.Distance(t.position, transform.position);
            if (d < minDist)
            {
                minDist = d;
                currentVent = t;
            }
        }
        float minDistY = float.MaxValue;
        foreach (var t in allVents)
        {
            if (t == currentVent) continue;

            float d = Mathf.Abs(t.position.y - transform.position.y);
            if (d < minDistY)
            {
                minDistY = d;
                nextVent = t;
            }
        }
    }

    void CheckAttack()
    {
        var hits = Physics2D.OverlapCircleAll(attackPos.position, attackRadius);
        foreach (var hit in hits)
        {
            var health = hit.GetComponent<HealthSystem>();
            if ((health) && (!health.isDead) && (health.faction == HealthSystem.Faction.Friendly))
            {
                health.DealDamage(attackDamage, attackPos.position);
            }
        }
    }

    void SnapToFloor()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, 50, groundMask);
        if (hit)
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }

    void RestartMovement()
    {
        canMove = true;
    }

    public void Kill()
    {
        if (dead) return;

        StartCoroutine(KillCR());
    }

    IEnumerator KillCR()
    { 
        // Kill alien
        animator.SetTrigger("Hit");
        spriteEffect.FlashInvert(0.2f);
        dead = true;
        rb.linearVelocityX = 0;
        deathPS.Play();
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.FadeTo(new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.1f).Done(() => spriteRenderer.enabled = false);

        yield return new WaitForSeconds(2.0f);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPos.position, attackRadius);
        }
    }
}
