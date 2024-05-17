using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossRun : StateMachineBehaviour
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


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
        bossAttack = animator.GetComponent<BossAttack>();
        knockBack = animator.GetComponent<Knockback>();
        capsuleCollider2d = animator.GetComponent<CapsuleCollider2D>();
        impulseSource = animator.GetComponent<CinemachineImpulseSource>();

        if (boss.isFacingRight)
        {
            boss.Flip();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastPlayerPosition = player.position;
        if (boss.inZone)
        {
            float distanceToPlayer = Vector2.Distance(animator.transform.position, player.position);
            Vector2 direction = (player.position - animator.transform.position).normalized;

            if (direction.x < 0 && boss.isFacingRight)
                 boss.Flip();
            else if (direction.x > 0 && !boss.isFacingRight)
                 boss.Flip();

            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            if (distanceToPlayer < attackRange && canAttack)
            {
                animator.SetTrigger("Attack");
            }
            if (distanceToPlayer > chaseRange)
            {
                // E�er oyuncu chaseRange'in d���nda ise ve daha �nce belirlenen hedefe ula��lmad�ysa
                if (!isJumping)
                {
                    JumpToLastPlayerPosition(animator);
                }
            }
            else
            {
                isJumping = false; // E�er oyuncu chaseRange i�indeyse, z�plama bitmi� olur.
            }
            Fall(animator);
            KnockbackController(animator);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }

    void JumpToLastPlayerPosition(Animator animator)
    {
        isJumping = true;
        // Yatayda hareket et
        float targetX = lastPlayerPosition.x + (boss.isFacingRight ? -jumpDistance : jumpDistance);
        float targetY = lastPlayerPosition.y + jumpHeight;

        // Hedefe do�ru z�pla
        Vector2 jumpTarget = new Vector2(targetX, targetY);
        rb.velocity = CalculateJumpVelocity(animator.transform.position, jumpTarget, 1f);
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

    void Fall(Animator animator)
    {
        // Karakterin yerde olup olmad���n� kontrol et
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


        // Karakter yerdeyse, z�plama durumunu s�f�rla
        if (rb.velocity.y != 0 && isGrounded)
        {
            impulseSource.GenerateImpulse();

            // Patlama yar��ap� ve etkile�ime girecek objeleri belirleme
            Collider2D[] colliders = Physics2D.OverlapCircleAll(animator.transform.position, fallDamageRadius, playerLayer);

            foreach (Collider2D collider in colliders)
            {
                // E�er d��man layer'�na sahip bir objeyle temas edildiyse
                PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // Hasar verme i�lemini ger�ekle�tir
                    playerHealth.TakeDamage(20);
                }
            }
        }
    }
    void KnockbackController(Animator animator)
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

