using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public float currentHealth;
    private int healthBarRange = 1;
    private Animator animator;
    private CinemachineImpulseSource impulseSource;
    private Knockback knockback;
    private Transform player;
    public Slider healthSlider;
    public bool isDied = false;
    [HideInInspector]public bool isDetected = true;

    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        knockback = GetComponent<Knockback>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        // Baþlangýçta saðlýk deðerini ayarlayýn
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

    }

    private void Update()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("TakeDamage");
        impulseSource.GenerateImpulse();
        Vector2 knockbackDirection = (player.position - transform.position).normalized;
        knockback.knockbackDirection = -1 * knockbackDirection;
        knockback.ApplyKnockback();
        // Saðlýk deðerini güncelle
        healthSlider.value = currentHealth;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, healthBarRange);
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        FindObjectOfType<SpawnManager>().EnemyKilled();

        gameObject.layer = LayerMask.NameToLayer("Died");

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Died");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        rb.velocity = Vector2.zero;
        isDied = true;
        Destroy(gameObject, 2f);
    }
}
