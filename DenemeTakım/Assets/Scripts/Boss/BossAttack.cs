using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private bool isBPressed = false;

    private Knockback knockBack;
    private Animator animator;

    [SerializeField] private int damageGiven;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask playerLayer;
    // Start is called before the first frame update
    void Start()
    {
        knockBack = GetComponent<Knockback>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

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
