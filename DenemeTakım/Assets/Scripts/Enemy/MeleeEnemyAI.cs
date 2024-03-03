using System.Collections;
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

    [SerializeField] private bool isFacingRight = true; // Karakterin saða dönük olup olmadýðý


    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float attackCooldown = 1f;
    private float currentMoveSpeed;
    private Animator animator;

    [SerializeField] private float initialMoveSpeed = 5f; // Baþlangýçtaki hýz
    [SerializeField] private float maxMoveSpeed = 15f; // Maksimum hýz

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = initialMoveSpeed;

        // Düþmanýn hangi yöne bakacaðýný ayarla
        if (!isFacingRight)
            transform.rotation = Quaternion.Euler(0, 180, 0); // Sola bakýyorsa
        else
            transform.rotation = Quaternion.Euler(0, 0, 0); // Saða bakýyorsa
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Eðer düþman oyuncunun menzilinde ve doðrudan hattý varsa ve saldýrmýyorsa
        if (distanceToPlayer <= chaseRange && !isAttacking && HasLineOfSight())
        {
            // Oyuncuyu takip et
            Vector2 direction = (player.position - transform.position).normalized;

            // Karakterin yönünü belirle
            if (direction.x < 0)
                transform.rotation = Quaternion.Euler(0, 0, 0); // Saða bakýyorsa
            else
                transform.rotation = Quaternion.Euler(0, 180, 0); // Sola bakýyorsa

            // Rigidbody'nin hýzýný ayarla
            rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
        }
        else
        {
            // Eðer düþman oyuncunun menzilinde deðilse veya doðrudan hattý yoksa, dur
            rb.velocity = Vector2.zero;
        }

        // Oyuncu saldýrý menziline girdiyse ve saldýrmýyorsa
        if (distanceToPlayer <= attackRange && !isAttacking && HasLineOfSight())
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
    private bool HasLineOfSight()
    {
        // Düþmanýn ve oyuncunun pozisyonlarýný al
        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = player.position;

        // Düþman ile oyuncu arasýnda bir hattýn olup olmadýðýný kontrol et
        RaycastHit2D hit = Physics2D.Linecast(enemyPosition, playerPosition, obstacleLayerMask);

        // Eðer hiçbir engel yoksa, yani doðrudan bir hattý varsa, true döndür
        return (hit.collider == null);
    }

    // Gizmos kullanarak attackPoint'i göster
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
