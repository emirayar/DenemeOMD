
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public float healthBarRange = 1f;
    private Animator animator;
    private CinemachineImpulseSource impulseSource;
    private Knockback knockback;
    private Transform player;

    [HideInInspector] public bool isDied = false;
    [HideInInspector] public bool isDetected = true;

    private Rigidbody2D rb;

    // Health bar de�i�kenleri
    [SerializeField] private float chipSpeed = 150f;
    private float lerptimer;
    [SerializeField] private Image frontHealthBar;
    [SerializeField] private Image BackHealthBar;

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

        // UI'yi g�nceller
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        // �n ve arka sa�l�k �ubuklar�n�n doluluk oranlar� al�n�r.
        float fillForeBar = frontHealthBar.fillAmount;
        float fillBackBar = BackHealthBar.fillAmount;
        float hFraction = currentHealth / maxHealth;
        if (fillBackBar > hFraction)
        {
            // �n sa�l�k �ubu�u, mevcut sa�l�k oran�na e�itlenene kadar yava��a azalt�l�r.
            // Bu i�lem, sa�l�k �ubuklar�n�n e� zamanl� olarak g�ncellenmesini sa�lar.
            frontHealthBar.fillAmount = hFraction;

            // Arka sa�l�k �ubu�u rengi beyaz olarak ayarlan�r.
            BackHealthBar.color = Color.white;

            // Zamanlay�c�, doluluk oran�n�n yava��a g�ncellenmesini sa�lar.
            lerptimer += Time.deltaTime;

            // Zaman�n ge�mesine g�re, �ubu�un ne kadar doldurulaca�� belirlenir.
            float percentComplete = lerptimer / chipSpeed;

            // Arka sa�l�k �ubu�unun doluluk oran�, mevcut doluluk oran�ndan yeni doluluk oran�na do�ru yumu�ak bir ge�i�le g�ncellenir.
            BackHealthBar.fillAmount = Mathf.Lerp(fillBackBar, hFraction, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("TakeDamage");
        impulseSource.GenerateImpulse();
        Vector2 knockbackDirection = (player.position - transform.position).normalized;
        knockbackDirection.y = 0.1f;
        knockback.knockbackDirection.x = -1 * knockbackDirection.x;
        knockback.ApplyKnockback();
        lerptimer = 0f;
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