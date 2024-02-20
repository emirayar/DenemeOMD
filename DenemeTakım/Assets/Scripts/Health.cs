using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;
    private CinemachineImpulseSource impulseSource;
    private Knockback knockback;
    private Transform player;
    public Slider healthSlider; // Saðlýk çubuðu

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        knockback = GetComponent<Knockback>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Saðlýk çubuðunu bulmak için slider'ý arayýn
        healthSlider = GameObject.FindWithTag("HealthSlider").GetComponent<Slider>();

        // Baþlangýçta saðlýk deðerini ayarlayýn
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
    private void Update()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
    public void TakeDamage(int damage)
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