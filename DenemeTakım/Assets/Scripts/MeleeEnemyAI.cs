using System.Collections;
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

    [SerializeField] private bool isFacingRight = true; // Karakterin sa�a d�n�k olup olmad���


    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float attackCooldown = 1f;
    private float currentMoveSpeed;
    private Animator animator;

    [SerializeField] private float initialMoveSpeed = 5f; // Ba�lang��taki h�z
    [SerializeField] private float maxMoveSpeed = 15f; // Maksimum h�z

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = initialMoveSpeed;

        // D��man�n hangi y�ne bakaca��n� ayarla
        if (!isFacingRight)
            transform.rotation = Quaternion.Euler(0, 180, 0); // Sola bak�yorsa
        else
            transform.rotation = Quaternion.Euler(0, 0, 0); // Sa�a bak�yorsa
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // E�er d��man oyuncunun menzilinde ve do�rudan hatt� varsa ve sald�rm�yorsa
        if (distanceToPlayer <= chaseRange && !isAttacking && HasLineOfSight())
        {
            // Oyuncuyu takip et
            Vector2 direction = (player.position - transform.position).normalized;

            // Karakterin y�n�n� belirle
            if (direction.x < 0)
                transform.rotation = Quaternion.Euler(0, 0, 0); // Sa�a bak�yorsa
            else
                transform.rotation = Quaternion.Euler(0, 180, 0); // Sola bak�yorsa

            // Rigidbody'nin h�z�n� ayarla
            rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
        }
        else
        {
            // E�er d��man oyuncunun menzilinde de�ilse veya do�rudan hatt� yoksa, dur
            rb.velocity = Vector2.zero;
        }

        // Oyuncu sald�r� menziline girdiyse ve sald�rm�yorsa
        if (distanceToPlayer <= attackRange && !isAttacking && HasLineOfSight())
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
    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    public void MoveForwardDuringAttack()
    {
        if (rb.velocity.x < 0.01f)
        {
            float horizontalSpeed = currentMoveSpeed * Mathf.Sign(transform.localScale.x);
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
    private bool HasLineOfSight()
    {
        // D��man�n ve oyuncunun pozisyonlar�n� al
        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = player.position;

        // D��man ile oyuncu aras�nda bir hatt�n olup olmad���n� kontrol et
        RaycastHit2D hit = Physics2D.Linecast(enemyPosition, playerPosition, obstacleLayerMask);

        // E�er hi�bir engel yoksa, yani do�rudan bir hatt� varsa, true d�nd�r
        return (hit.collider == null);
    }

    // Gizmos kullanarak attackPoint'i g�ster
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
