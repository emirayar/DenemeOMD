using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : MonoBehaviour
{
    private Transform player; // Oyuncunun pozisyonunu tutacak transform
    [SerializeField] private float chaseRange = 10f; // Takip menzili
    [SerializeField] private float attackRange = 2f; // Saldýrý menzili
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
    private LineOfSight lineOfSight;
    private bool isFacingRight;

    [SerializeField] private float initialMoveSpeed = 5f; // Baþlangýçtaki hýz
    [SerializeField] private float maxMoveSpeed = 15f; // Maksimum hýz

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = initialMoveSpeed;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        lineOfSight = GetComponent<LineOfSight>();

        // Düþmanýn hangi yöne bakacaðýný ayarla
        if (transform.position.x > player.position.x)
            Flip(); // Saða bakýyorsa
    }

    private void Update()
    {
        if (lineOfSight.visibleTargets.Count > 0)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Eðer düþman oyuncunun menzilinde ve doðrudan hattý varsa ve saldýrmýyorsa
            if (distanceToPlayer <= chaseRange && !isAttacking)
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
            }
            else
            {
                // Eðer düþman oyuncunun menzilinde deðilse, dur
                rb.velocity = Vector2.zero;
            }

            // Oyuncu saldýrý menziline girdiyse ve saldýrmýyorsa
            if (distanceToPlayer <= attackRange && !isAttacking)
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
        if (rb.velocity.x < 0.01f)
        {
            float horizontalSpeed = currentMoveSpeed * (isFacingRight ? 1 : -1);
            rb.velocity = new Vector2(horizontalSpeed, rb.velocity.y);
        }
    }

    // Saldýrýyý gerçekleþtir
    private void GiveDamage()
    {
        // Düþmanýn saldýrý mesafesine girdiði konumda saldýrý yap
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(damageGiven);
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
