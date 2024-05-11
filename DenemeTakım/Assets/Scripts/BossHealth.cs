using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    private Animator animator;
    private CinemachineImpulseSource impulseSource;
    private Knockback knockback;
    private Transform player;

    [HideInInspector] public bool isDied = false;
    [HideInInspector] public bool isDetected = true;

    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        knockback = GetComponent<Knockback>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
    public void BossTakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("TakeDamage");
        impulseSource.GenerateImpulse();
        Vector2 knockbackDirection = (player.position - transform.position).normalized;
        knockbackDirection.y = 0.1f;
        knockback.knockbackDirection.x = -1 * knockbackDirection.x;
        knockback.ApplyKnockback();
        if (currentHealth <= 0)
        {
            BossDie();
        }
    }
    private void BossDie()
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