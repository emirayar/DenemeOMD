using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100; //D��man�n maksimum sa�l���
    private int currentHealth;   // Mevcut sa�l�k

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth; // Mevcut sa�l��� maksimum sa�l��a e�itle
        animator = GetComponent<Animator>();
    }

    // Hasar almay� i�le
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Hasar� mevcut sa�l�ktan ��kar
        animator.SetTrigger("TakeDamage");
        if (currentHealth <= 0)
        {
            Die(); // E�er sa�l�k s�f�rsa d��man� �ld�r
        }
    }

    // D��man�n �l�m�
    private void Die()
    {
        animator.SetTrigger("Die");
        // D��man �ld���nde SpawnManager'a haber ver
        FindObjectOfType<SpawnManager>().EnemyKilled();
        Destroy(gameObject, 2f); // D��man� yok et (2 saniye sonra)
    }
}
