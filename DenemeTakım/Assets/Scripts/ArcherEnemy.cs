using UnityEngine;
using System.Collections;

public class ArcherEnemy : MonoBehaviour
{
    private Transform player;
    public float chaseRange = 5f; // Kovalama menzili
    public float attackRange = 2f; // Sald�r� menzili
    public float attackCooldown = 2f; // Sald�r�lar aras�ndaki bekleme s�resi
    public GameObject arrowPrefab; // Ok prefab�
    public Transform firePoint; // Okun ate�lenece�i nokta
    public float arrowForce = 0f; // Okun hedefe do�ru uygulanacak kuvvet
    public float arrowHeightOffset = 0.1f; // Okun karakterin �st�nden ge�mesi i�in ba�lang�� y�kseklik fark�

    private Animator animator; // Karakterin animat�r bile�eni
    private bool isAttacking = false; // Sald�r� durumunu kontrol etmek i�in

    private void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("CheckDistance", 0f, 0.5f); // Her yar�m saniyede bir uzakl��� kontrol et
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void CheckDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Oyuncu kovalama menziline girdiyse ve sald�rm�yorsa
        if (distanceToPlayer <= chaseRange && !isAttacking)
        {
            // Oyuncuyu takip et
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime);

            // Karakterin y�n�n� belirle
            if (direction.x > 0)
                transform.rotation = Quaternion.Euler(0, 0, 0); // Sa�a bak�yorsa
            else
                transform.rotation = Quaternion.Euler(0, 180, 0); // Sola bak�yorsa
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
