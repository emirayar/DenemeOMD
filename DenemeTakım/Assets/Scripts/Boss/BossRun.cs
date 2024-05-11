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
    private Boss boss;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
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
            // Oyuncuyu takip et
            Vector2 direction = (player.position - animator.transform.position).normalized;

            // Karakterin yönünü belirle
            if (direction.x < 0 && boss.isFacingRight)
                boss.Flip(); // Saða bakýyorsa
            else if (direction.x > 0 && !boss.isFacingRight)
                boss.Flip(); // Sola bakýyorsa

            // Rigidbody'nin hýzýný ayarla
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            float distanceToPlayer = Vector2.Distance(animator.transform.position, player.position);
            if (distanceToPlayer < attackRange)
            {
                animator.SetTrigger("Attack");
            }
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.ResetTrigger("Attack");
    }
}
