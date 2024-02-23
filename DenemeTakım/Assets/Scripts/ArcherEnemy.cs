using UnityEngine;
using System.Collections;

public class ArcherEnemy : MonoBehaviour
{
    private Transform player;
    public float chaseRange = 5f; // Kovalama menzili
    public float attackRange = 2f; // Saldýrý menzili
    public float attackCooldown = 2f; // Saldýrýlar arasýndaki bekleme süresi
    public GameObject arrowPrefab; // Ok prefabý
    public Transform firePoint; // Okun ateþleneceði nokta
    public float arrowForce = 0f; // Okun hedefe doðru uygulanacak kuvvet
    public float arrowHeightOffset = 0.1f; // Okun karakterin üstünden geçmesi için baþlangýç yükseklik farký

    private Animator animator; // Karakterin animatör bileþeni
    private bool isAttacking = false; // Saldýrý durumunu kontrol etmek için

    private void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("CheckDistance", 0f, 0.5f); // Her yarým saniyede bir uzaklýðý kontrol et
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void CheckDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Oyuncu kovalama menziline girdiyse ve saldýrmýyorsa
        if (distanceToPlayer <= chaseRange && !isAttacking)
        {
            // Oyuncuyu takip et
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime);

            // Karakterin yönünü belirle
            if (direction.x > 0)
                transform.rotation = Quaternion.Euler(0, 0, 0); // Saða bakýyorsa
            else
                transform.rotation = Quaternion.Euler(0, 180, 0); // Sola bakýyorsa
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
