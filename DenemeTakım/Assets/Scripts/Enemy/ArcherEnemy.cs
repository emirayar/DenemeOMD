using UnityEngine;
using System.Collections;

public class ArcherEnemy : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float chaseRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float chaseSpeed;

    [SerializeField] private float attackCooldown = 2f; // Saldýrýlar arasýndaki bekleme süresi
    [SerializeField] private GameObject arrowPrefab; // Ok prefabý
    [SerializeField] private Transform firePoint; // Okun ateþleneceði nokta
    [SerializeField] private float arrowForce; // Okun hedefe doðru uygulanacak kuvvet
    [SerializeField] private float arrowHeightOffset; // Okun karakterin üstünden geçmesi için baþlangýç yükseklik farký
    [SerializeField] private LayerMask obstacleLayerMask;

    private Animator animator; // Karakterin animatör bileþeni
    private bool isAttacking = false; // Saldýrý durumunu kontrol etmek için

    private Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("CheckDistance", 0f, 0.5f); // Her yarým saniyede bir uzaklýðý kontrol et
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void CheckDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Eðer düþman oyuncunun menzilinde ve doðrudan hattý varsa ve saldýrmýyorsa
        if (distanceToPlayer <= chaseRange && !isAttacking && HasLineOfSight())
        {
            // Oyuncuyu takip et
            Vector2 direction = (player.position - transform.position).normalized;

            // Karakterin yönünü belirle
            if (direction.x > 0)
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

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false; // Saldýrý durumunu sýfýrla
    }

    // Bu metod, animasyon eventinden çaðrýlacak
    public void FireArrow()
    {
        // Ok prefabýndan bir klon oluþtur
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

        arrow.transform.position += new Vector3(0, arrowHeightOffset, 0);

        // Ok klonunu hareket ettirme (örneðin, rigitbody kullanarak)
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        // Hedefe doðru kuvvet uygula
        Vector2 direction = (player.position - firePoint.position).normalized;
        rb.AddForce(direction * arrowForce, ForceMode2D.Impulse);
    }

}
