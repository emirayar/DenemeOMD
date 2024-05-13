using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Vector2 lastPlayerPosition;
    private bool isJumping = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
        lastPlayerPosition = player.position;
        if (boss.isFacingRight)
        {
            boss.Flip();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss.inZone)
        {
            Vector2 direction = (player.position - animator.transform.position).normalized;

            if (direction.x < 0 && boss.isFacingRight)
                boss.Flip();
            else if (direction.x > 0 && !boss.isFacingRight)
                boss.Flip();

            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            float distanceToPlayer = Vector2.Distance(animator.transform.position, player.position);
            if (distanceToPlayer < attackRange)
            {
                animator.SetTrigger("Attack");
            }
            else if (distanceToPlayer > chaseRange)
            {
                // Eðer oyuncu chaseRange'in dýþýnda ise ve daha önce belirlenen hedefe ulaþýlmadýysa
                if (!isJumping)
                {
                    JumpToLastPlayerPosition(animator);
                }
            }
            else
            {
                isJumping = false; // Eðer oyuncu chaseRange içindeyse, zýplama bitmiþ olur.
            }
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

        // Hedefe doðru zýpla
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
}
