using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2.0f;
    public float detectionRadius = 5.0f;
    public float attackRange = 2.0f;
    public float attackCooldown = 1.0f;
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.0f;
    public Animator animator;

    private int currentPatrolPointIndex = 0;
    private bool isPatrolling = true;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    private float attackCooldownTimer = 0.0f;

    private void Update()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(target.position, transform.position);

            if (isPatrolling)
            {
                if (distanceToTarget <= detectionRadius)
                {
                    isPatrolling = false;
                }
                else
                {
                    Patrol();
                }
            }
            else
            {
                if (distanceToTarget > detectionRadius)
                {
                    isPatrolling = true;
                    isAttacking = false;
                }
                else
                {
                    Chase();
                }
            }
        }
        
        if (isAttacking)
        {
            attackCooldownTimer += Time.deltaTime;
            if (attackCooldownTimer >= attackCooldown)
            {
                isAttacking = false;
                animator.SetTrigger("CombatIdle");
                attackCooldownTimer = 0.0f;
            }
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length > 0)
        {
            Vector3 targetPoint = patrolPoints[currentPatrolPointIndex].position;
            Vector3 moveDirection = (targetPoint - transform.position).normalized;

            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
                Flip();
            }

            transform.Translate(moveDirection * patrolSpeed * Time.deltaTime);
            animator.SetFloat("Speed", Mathf.Abs(moveDirection.x));
        }
    }

    private void Chase()
    {
        Vector3 moveDirection = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (distanceToTarget <= attackRange)
        {
            Attack();
        }
        else
        {
            isAttacking = false;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            if (moveDirection.x > 0 && isFacingRight)
            {
                Flip();
            }
            else if (moveDirection.x < 0 && !isFacingRight)
            {
                Flip();
            }

            animator.SetFloat("Speed", Mathf.Abs(moveDirection.x));
        }
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
