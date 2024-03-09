using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : MonoBehaviour
{
    private Transform player; // Oyuncunun pozisyonunu tutacak transform
    [SerializeField] private float chaseRange = 10f; // Takip menzili
    [SerializeField] private float attackRange = 2f; // Sald�r� menzili
    [SerializeField] private float chaseSpeed = 5f; // Hareket h�z�
    [SerializeField] private Transform attackPoint; // Sald�r� yap�lacak nokta
    [SerializeField] private LayerMask playerLayer; // Oyuncu katman�
    [SerializeField] private int damageGiven;
    [SerializeField] private LayerMask obstacleLayerMask;

    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float attackCooldown = 1f;
    private float currentMoveSpeed;
    private Animator animator;
    private LineOfSight lineOfSight;
    private bool isFacingRight;

    [SerializeField] private float initialMoveSpeed = 5f; // Ba�lang��taki h�z
    [SerializeField] private float maxMoveSpeed = 15f; // Maksimum h�z

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = initialMoveSpeed;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        lineOfSight = GetComponent<LineOfSight>();

        // D��man�n hangi y�ne bakaca��n� ayarla
        if (transform.position.x > player.position.x)
            Flip(); // Sa�a bak�yorsa
    }

    private void Update()
    {
        if (lineOfSight.visibleTargets.Count > 0)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // E�er d��man oyuncunun menzilinde ve do�rudan hatt� varsa ve sald�rm�yorsa
            if (distanceToPlayer <= chaseRange && !isAttacking)
            {
                // Oyuncuyu takip et
                Vector2 direction = (player.position - transform.position).normalized;

                // Karakterin y�n�n� belirle
                if (direction.x < 0 && isFacingRight)
                    Flip(); // Sa�a bak�yorsa
                else if (direction.x > 0 && !isFacingRight)
                    Flip(); // Sola bak�yorsa

                // Rigidbody'nin h�z�n� ayarla
                rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
            }
            else
            {
                // E�er d��man oyuncunun menzilinde de�ilse, dur
                rb.velocity = Vector2.zero;
            }

            // Oyuncu sald�r� menziline girdiyse ve sald�rm�yorsa
            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                // Sald�r� animasyonunu oynat
                animator.SetTrigger("Attack");
                // Sald�r� durumunu i�aretle
                isAttacking = true;
                // Sald�r� animasyonunun s�resi boyunca beklemek i�in coroutine ba�lat
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

    // Sald�r�y� ger�ekle�tir
    private void GiveDamage()
    {
        // D��man�n sald�r� mesafesine girdi�i konumda sald�r� yap
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(damageGiven);
        }
    }

    // Karakterin y�n�n� de�i�tir
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Gizmos kullanarak attackPoint'i g�ster
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
