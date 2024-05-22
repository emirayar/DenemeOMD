using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossRun : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float chaseRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpDistance;
    private Boss boss;
    private BossAttack bossAttack;
    private Knockback knockBack;
    public Vector2 lastPlayerPosition;
    private bool isJumping = false;
    private CinemachineImpulseSource impulseSource;

    private CapsuleCollider2D capsuleCollider2d;
    [SerializeField] private LayerMask groundlayerMask;
    [SerializeField] private LayerMask playerLayer;
    private bool isGrounded;
    [SerializeField] private float fallDamageRadius;
    private bool canAttack = true;

    private Animator animator;
    private bool damageDealt = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        boss = GetComponent<Boss>();
        bossAttack = GetComponent<BossAttack>();
        knockBack = GetComponent<Knockback>();
        capsuleCollider2d = GetComponent<CapsuleCollider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        animator = GetComponent<Animator>();

        if (boss.isFacingRight)
        {
            boss.Flip();
        }
    }

    void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        lastPlayerPosition = player.position;
        if (boss.inZone)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0 && boss.isFacingRight)
                boss.Flip();
            else if (direction.x > 0 && !boss.isFacingRight)
                boss.Flip();

            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            if (distanceToPlayer < attackRange && canAttack)
            {
                rb.velocity = Vector2.zero;
                bossAttack.PerformAttack();
            }
            if (distanceToPlayer > chaseRange)
            {
                if (!isJumping)
                {
                    JumpToLastPlayerPosition();
                }
            }
            else
            {
                isJumping = false;
            }

            if(rb.velocity.y < 0)
            {
                animator.SetBool("isFalling", true);
            }
            else
            {
                animator.SetBool("isFalling", false);
            }

            GroundImpact();
            KnockbackController();
        }
    }

    void JumpToLastPlayerPosition()
    {
        isJumping = true;
        float targetX = lastPlayerPosition.x + (boss.isFacingRight ? -jumpDistance : jumpDistance);
        float targetY = lastPlayerPosition.y + jumpHeight;

        Vector2 jumpTarget = new Vector2(targetX, targetY);
        rb.velocity = CalculateJumpVelocity(transform.position, jumpTarget, 1f);
    }

    Vector2 CalculateJumpVelocity(Vector2 origin, Vector2 target, float timeToTarget)
    {
        float gravity = -Physics2D.gravity.y;
        float y = target.y - origin.y;
        Vector2 xzTarget = new Vector2(target.x, 0) - new Vector2(origin.x, 0);
        float xzDist = xzTarget.magnitude;
        float yVelocity = (y + 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;
        float xzVelocity = xzDist / timeToTarget;

        Vector2 result = xzTarget.normalized;
        result *= xzVelocity;
        result.y = yVelocity;

        return result;
    }

    void GroundImpact()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(capsuleCollider2d.bounds.center, Vector2.down, capsuleCollider2d.bounds.extents.y + 0.3f, groundlayerMask);

        Color rayColor;

        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(capsuleCollider2d.bounds.center, Vector2.down * (capsuleCollider2d.bounds.extents.y + 0.2f), rayColor);
        isGrounded = raycastHit.collider != null;

        if (rb.velocity.y != 0 && isGrounded && !damageDealt)
        {
            impulseSource.GenerateImpulse();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, fallDamageRadius, playerLayer);

            foreach (Collider2D collider in colliders)
            {
                PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(20);
                    damageDealt = true;
                    break;
                }
            }
        }
    }

    void KnockbackController()
    {
        if (knockBack.isKnockbackked)
        {
            animator.SetBool("isKnockbackked", true);
            rb.velocity = Vector2.zero;
            canAttack = false;
        }
        else
        {
            animator.SetBool("isKnockbackked", false);
            canAttack = true;
        }
    }
}
