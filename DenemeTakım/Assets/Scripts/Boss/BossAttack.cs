using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private bool isBPressed = false;
    private bool isAttacking = false;
    private int attackCount = 1;

    private Knockback knockBack;
    private Animator animator;

    [SerializeField] private int damageGiven;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float shortCooldown = 1f;
    [SerializeField] private float longCooldown = 3f;

    void Start()
    {
        knockBack = GetComponent<Knockback>();
        animator = GetComponent<Animator>();
    }

    private void CheckParry()
    {
        if (Input.GetButton("Block"))
        {
            isBPressed = true;
        }
        else
        {
            isBPressed = false;
        }
    }

    public void PerformAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("Attack" + attackCount);
        attackCount++;

        if (attackCount >= 3)
        {
            attackCount = 1;
            yield return new WaitForSeconds(longCooldown);
        }
        else
        {
            yield return new WaitForSeconds(shortCooldown);
        }
        isAttacking = false;
    }

    private void GiveDamage()
    {
        // Düþmanýn saldýrý mesafesine girdiði konumda saldýrý yap
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            if (!isBPressed && Input.GetButton("Block"))
            {
                player.GetComponent<Block>().Parry();
                animator.SetTrigger("TakeDamage");
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                knockBack.knockbackDirection.x = -1 * knockbackDirection.x;
                knockBack.ApplyKnockback();
            }
            else
            {
                if (player.GetComponent<Block>().isBlocking)
                {
                    player.GetComponent<Stamina>().UseStamina(30f);
                }
                if (!player.GetComponent<Block>().isBlocking || player.GetComponent<Stamina>().currentStamina < 15f)
                {
                    player.GetComponent<PlayerHealth>().TakeDamage(damageGiven);
                }
                player.GetComponent<Knockback>().knockbackDirection = (transform.position - player.transform.position).normalized;
                player.GetComponent<Knockback>().knockbackDirection.y = 0.1f;
                player.GetComponent<Knockback>().knockbackDirection.x = -1 * player.GetComponent<Knockback>().knockbackDirection.x;
                player.GetComponent<Knockback>().ApplyKnockback();
            }
        }
    }
}
