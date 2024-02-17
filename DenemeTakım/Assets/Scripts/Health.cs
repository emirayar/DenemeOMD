using UnityEngine;
using Cinemachine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;
    private CinemachineImpulseSource impulseSource;
    private Knockback knockback;
    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        knockback = GetComponent<Knockback>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("TakeDamage");
        impulseSource.GenerateImpulse();
        Vector2 knockbackDirection = (player.position - transform.position).normalized; // Oyuncunun konumundan düþmanýn konumunu çýkar ve normalleþtir
        knockback.knockbackDirection = -1*knockbackDirection; // Knockback yönünü belirle
        knockback.ApplyKnockback(); // Knockback uygula
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        FindObjectOfType<SpawnManager>().EnemyKilled();

        gameObject.layer = LayerMask.NameToLayer("Died");

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Died");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        Destroy(gameObject, 2f);
    }
}
