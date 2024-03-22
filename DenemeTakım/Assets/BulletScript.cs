using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // PlayerHealth türünde bir deðiþken tanýmlayýn
    public PlayerHealth playerHealth;

    void Start()
    {

    }

    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Temas edilen nesnenin "Player" etiketine sahip olup olmadýðýný kontrol edin
        if (collision.gameObject.CompareTag("Player"))
        {
            // Temas edilen nesne bir oyuncuysa, oyuncunun saðlýðýný azaltýn
            playerHealth.TakeDamage(10); // Örnek olarak, oyuncunun saðlýðýný 10 azalttýk
        }

        // Bullet nesnesini yok edin
        Destroy(gameObject);
    }
}

