using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : MonoBehaviour
{
    private Transform player; // Oyuncunun pozisyonunu tutacak transform
    [SerializeField] private float chaseRange = 10f; // Takip menzili
    [SerializeField] private float attackRange = 2f; // Saldýrý menzili
    [SerializeField] private float attackRadius; // Saldýrý menzili
    [SerializeField] private float chaseSpeed = 5f; // Hareket hýzý
    [SerializeField] private Transform attackPoint; // Saldýrý yapýlacak nokta
    [SerializeField] private LayerMask playerLayer; // Oyuncu katmaný
    [SerializeField] private int damageGiven;
    [SerializeField] private LayerMask obstacleLayerMask;

    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float attackCooldown = 1f;
    private float currentMoveSpeed;
    private Animator animator;
    public bool isFacingRight;
    public bool isAggressive = false; // Düþmanýn agresif takip durumu
    private bool isBPressed = false;

    [SerializeField] private float initialMoveSpeed = 5f; // Baþlangýçtaki hýz
    [SerializeField] private float maxMoveSpeed = 15f; // Maksimum hýz

    private Health health;
    private Knockback knockBack;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = initialMoveSpeed;
        health = GetComponent<Health>();
        knockBack = GetComponent<Knockback>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (isFacingRight)
        {
            Flip();
        }
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && health.currentHealth != health.maxHealth && !health.isDied)
        {
            isAggressive = true;
        }

        if (isAggressive || isAttacking)
        {
            //Eðer düþman oyuncunun menzilinde veya agresif takip durumunda ve saldýrmýyorsa ve ölmediyse.
            if ((distanceToPlayer <= chaseRange || isAggressive) && !isAttacking && !health.isDied)
            {
                if (!knockBack.isKnockbackked)
                {
                    // Oyuncuyu takip et
                    Vector2 direction = (player.position - transform.position).normalized;

                    // Karakterin yönünü belirle
                    if (direction.x < 0 && isFacingRight)
                        Flip(); // Saða bakýyorsa
                    else if (direction.x > 0 && !isFacingRight)
                        Flip(); // Sola bakýyorsa

                    // Rigidbody'nin hýzýný ayarla
                    rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
                    animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
                }

            }
            else
            {
                // Eðer düþman oyuncunun menzilinde deðilse, dur
                rb.velocity = Vector2.zero;
            }

            // Oyuncu saldýrý menziline girdiyse ve saldýrmýyorsa
            if (distanceToPlayer <= attackRange && !isAttacking && !health.isDied)
            {
                // Saldýrý animasyonunu oynat
                animator.SetTrigger("Attack");
                // Saldýrý durumunu iþaretle
                isAttacking = true;
                // Saldýrý animasyonunun süresi boyunca beklemek için coroutine baþlat
                StartCoroutine(ResetAttackCooldown());
            }
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, maxMoveSpeed, 0.01f);
        }
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    public void MoveForwardDuringAttack()
    {
        if (rb.velocity.x < 0.1f)
        {
            float horizontalSpeed = currentMoveSpeed * (isFacingRight ? 1 : -1);
            rb.velocity = new Vector2(horizontalSpeed, rb.velocity.y);
        }
    }
    private void CheckParry()
    {
        if (Input.GetButton("Block"))
        {
            isBPressed = true;
        }else
        {
            isBPressed = false;
        }
    }
    // Saldýrýyý gerçekleþtir
    private void GiveDamage()
    {
        if (!health.isDied)
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

    // Karakterin yönünü deðiþtir
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Gizmos kullanarak attackPoint'i göster
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
