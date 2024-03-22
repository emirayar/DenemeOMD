using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // PlayerHealth t�r�nde bir de�i�ken tan�mlay�n
    public PlayerHealth playerHealth;

    void Start()
    {

    }

    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Temas edilen nesnenin "Player" etiketine sahip olup olmad���n� kontrol edin
        if (collision.gameObject.CompareTag("Player"))
        {
            // Temas edilen nesne bir oyuncuysa, oyuncunun sa�l���n� azalt�n
            playerHealth.TakeDamage(10); // �rnek olarak, oyuncunun sa�l���n� 10 azaltt�k
        }

        // Bullet nesnesini yok edin
        Destroy(gameObject);
    }
}

