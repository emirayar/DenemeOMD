using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100; //Düþmanýn maksimum saðlýðý
    private int currentHealth;   // Mevcut saðlýk

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth; // Mevcut saðlýðý maksimum saðlýða eþitle
        animator = GetComponent<Animator>();
    }

    // Hasar almayý iþle
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Hasarý mevcut saðlýktan çýkar
        animator.SetTrigger("TakeDamage");
        if (currentHealth <= 0)
        {
            Die(); // Eðer saðlýk sýfýrsa düþmaný öldür
        }
    }

    // Düþmanýn ölümü
    private void Die()
    {
        animator.SetTrigger("Die");
        // Düþman öldüðünde SpawnManager'a haber ver
        FindObjectOfType<SpawnManager>().EnemyKilled();

        gameObject.layer = LayerMask.NameToLayer("Died");

        // Player ve enemyLayer'in carpismalarini gecici olarak ihmal et
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Died");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        Destroy(gameObject, 2f); // Düþmaný yok et (2 saniye sonra)
    }
}
