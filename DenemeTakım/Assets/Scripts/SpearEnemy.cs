using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearEnemy : MonoBehaviour
{
    public float attackRange = 3f; // D��man�n sald�r� menzili
    public float moveSpeed = 5f; // D��man�n hareket h�z�
    private Transform player; // Oyuncunun pozisyonunu saklamak i�in
    private Animator animator; // Karakterin animat�r bile�eni
    private bool isAttacking = false; // Sald�r� durumunu kontrol etmek i�in

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Oyuncunun pozisyonunu al
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // D��man�n oyuncuya do�ru hareket etmesi
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            // Sald�r� animasyonunu oynat
            animator.SetTrigger("Attack");
            // Sald�r� durumunu i�aretle
            isAttacking = true;
            
            
        }
    }

    void Attack()
    {
        // Burada sald�r� i�lemleri yap�labilir, �rne�in oyuncuya zarar verme, animasyon oynatma vb.
        Debug.Log("Sald�r�!");

        // �rnek olarak sadece Debug.Log kullan�ld�, ger�ek sald�r� i�lemleri buraya eklenebilir.
    }
}

