using UnityEngine;
using System.Collections;

public class ArcherEnemy : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float chaseRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float chaseSpeed;

    [SerializeField] private float attackCooldown = 2f; // Sald�r�lar aras�ndaki bekleme s�resi
    [SerializeField] private GameObject arrowPrefab; // Ok prefab�
    [SerializeField] private Transform firePoint; // Okun ate�lenece�i nokta
    [SerializeField] private float arrowForce; // Okun hedefe do�ru uygulanacak kuvvet
    [SerializeField] private float arrowHeightOffset; // Okun karakterin �st�nden ge�mesi i�in ba�lang�� y�kseklik fark�
    [SerializeField] private LayerMask obstacleLayerMask;

    private Animator animator; // Karakterin animat�r bile�eni
    private bool isAttacking = false; // Sald�r� durumunu kontrol etmek i�in

    private Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("CheckDistance", 0f, 0.5f); // Her yar�m saniyede bir uzakl��� kontrol et
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void CheckDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // E�er d��man oyuncunun menzilinde ve do�rudan hatt� varsa ve sald�rm�yorsa
        if (distanceToPlayer <= chaseRange && !isAttacking && HasLineOfSight())
        {
            // Oyuncuyu takip et
            Vector2 direction = (player.position - transform.position).normalized;

            // Karakterin y�n�n� belirle
            if (direction.x > 0)
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

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false; // Sald�r� durumunu s�f�rla
    }

    // Bu metod, animasyon eventinden �a�r�lacak
    public void FireArrow()
    {
        // Ok prefab�ndan bir klon olu�tur
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

        arrow.transform.position += new Vector3(0, arrowHeightOffset, 0);

        // Ok klonunu hareket ettirme (�rne�in, rigitbody kullanarak)
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        // Hedefe do�ru kuvvet uygula
        Vector2 direction = (player.position - firePoint.position).normalized;
        rb.AddForce(direction * arrowForce, ForceMode2D.Impulse);
    }

}
