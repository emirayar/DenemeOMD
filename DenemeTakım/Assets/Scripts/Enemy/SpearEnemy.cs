using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearEnemy : MonoBehaviour
{
    public float attackRange = 3f; // Düþmanýn saldýrý menzili
    public float moveSpeed = 5f; // Düþmanýn hareket hýzý
    private Transform player; // Oyuncunun pozisyonunu saklamak için
    private Animator animator; // Karakterin animatör bileþeni
    private bool isAttacking = false; // Saldýrý durumunu kontrol etmek için

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Oyuncunun pozisyonunu al
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // Düþmanýn oyuncuya doðru hareket etmesi
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            // Saldýrý animasyonunu oynat
            animator.SetTrigger("Attack");
            // Saldýrý durumunu iþaretle
            isAttacking = true;
            
            
        }
    }

    void Attack()
    {
        // Burada saldýrý iþlemleri yapýlabilir, örneðin oyuncuya zarar verme, animasyon oynatma vb.
        Debug.Log("Saldýrý!");

        // Örnek olarak sadece Debug.Log kullanýldý, gerçek saldýrý iþlemleri buraya eklenebilir.
    }
}

