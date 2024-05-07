
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

    // Health bar deðiþkenleri
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

        // UI'yi günceller
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        // Ön ve arka saðlýk çubuklarýnýn doluluk oranlarý alýnýr.
        float fillForeBar = frontHealthBar.fillAmount;
        float fillBackBar = BackHealthBar.fillAmount;
        float hFraction = currentHealth / maxHealth;
        if (fillBackBar > hFraction)
        {
            // Ön saðlýk çubuðu, mevcut saðlýk oranýna eþitlenene kadar yavaþça azaltýlýr.
            // Bu iþlem, saðlýk çubuklarýnýn eþ zamanlý olarak güncellenmesini saðlar.
            frontHealthBar.fillAmount = hFraction;

            // Arka saðlýk çubuðu rengi beyaz olarak ayarlanýr.
            BackHealthBar.color = Color.white;

            // Zamanlayýcý, doluluk oranýnýn yavaþça güncellenmesini saðlar.
            lerptimer += Time.deltaTime;

            // Zamanýn geçmesine göre, çubuðun ne kadar doldurulacaðý belirlenir.
            float percentComplete = lerptimer / chipSpeed;

            // Arka saðlýk çubuðunun doluluk oraný, mevcut doluluk oranýndan yeni doluluk oranýna doðru yumuþak bir geçiþle güncellenir.
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