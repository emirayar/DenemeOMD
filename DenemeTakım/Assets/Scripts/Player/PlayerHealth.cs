using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Variables")]
    // Maksimum saðlýk puaný
    public float maxHealth = 100f;

    // Oyuncunun mevcut saðlýk puaný
    private float currentHealth;

    [Header("Chip Variables")]
    // Saðlýk çubuðunun geçiþ hýzý
    [SerializeField] private float chipSpeed = 150f;

    //Lineer interpolasyon kullanarak çubuðun yavaþ yavaþ hareket etmesi için kullandýðýmýz timer
    private float lerptimer;

    private CinemachineImpulseSource impulseSource;

    [Header("Health Bars")]
    // Saðlýk çubuðunun önyüzü ve arka yüzü için Image bileþenleri
    [SerializeField] private Image frontHealthBar;
    [SerializeField] private Image BackHealthBar;

    // Baþlangýçta oyuncunun maksimum saðlýk deðerine sahip olmasýný saðlar.
    private void Start()
    {
        currentHealth = maxHealth;
        impulseSource = GetComponentInChildren<CinemachineImpulseSource> ();
    }

    // Her güncellemede saðlýk durumunu kontrol eder ve UI'yi günceller.
    private void Update()
    {
        // Saðlýk deðerini sýnýrlar
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // UI'yi günceller
        UpdateHealthUI();

        // H tuþuna basýldýðýnda, saðlýk deðerini artýrýr.
        if (Input.GetKeyDown(KeyCode.H))
        {
            RestoreHealth(10f);
        }
    }

    // Saðlýk UI'sýný günceller
    public void UpdateHealthUI()
    {
        // Ön ve arka saðlýk çubuklarýnýn doluluk oranlarý alýnýr.
        float fillForeBar = frontHealthBar.fillAmount;
        float fillBackBar = BackHealthBar.fillAmount;

        // Mevcut saðlýk oraný hesaplanýr.
        float hFraction = currentHealth / maxHealth;

        // Arka saðlýk çubuðunun doluluk oraný, mevcut saðlýk oranýna eþit olana kadar yavaþça güncellenir.
        // Eðer arka saðlýk çubuðunun doluluk oraný mevcut saðlýk oranýndan büyükse, çubuðun doluluk oraný yavaþça azaltýlýr.
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

        // Ön saðlýk çubuðunun doluluk oraný, mevcut saðlýk oranýna eþit olana kadar yavaþça güncellenir.
        if (fillForeBar < hFraction)
        {
            // Arka saðlýk çubuðunun rengi mavi olarak ayarlanýr.
            // Bu, oyuncuya saðlýk çubuðunun ön tarafýnda bir deðiþiklik olduðunu gösterir.
            BackHealthBar.color = Color.green;

            // Arka saðlýk çubuðunun doluluk oraný, mevcut saðlýk oranýna eþitlenir.
            BackHealthBar.fillAmount = hFraction;

            // Zamanlayýcý, doluluk oranýnýn yavaþça güncellenmesini saðlar.
            lerptimer += Time.deltaTime;

            // Zamanýn geçmesine göre, çubuðun ne kadar doldurulacaðý belirlenir.
            float percentComplete = lerptimer / chipSpeed;

            // Ön saðlýk çubuðunun doluluk oraný, arka saðlýk çubuðunun doluluk oranýna doðru yumuþak bir geçiþle güncellenir.
            frontHealthBar.fillAmount = Mathf.Lerp(fillForeBar, BackHealthBar.fillAmount, percentComplete);
        }

    }

    // Oyuncunun hasar almasýný saðlar
    public void TakeDamage(int damage)
    {
        // Mevcut saðlýk deðeri hasar kadar azaltýlýr.
        currentHealth -= damage;
        lerptimer = 0f;
        impulseSource.GenerateImpulse();

        // Saðlýk sýfýrlanýrsa, ölüm iþlevi çaðýrýlýr.
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Oyuncunun saðlýðýný artýrýr
    public void RestoreHealth(float healAmount)
    {
        // Mevcut saðlýk deðeri artýrýlýr.
        currentHealth += healAmount;
        lerptimer = 0f;
    }

    // Oyuncunun ölümü durumunda çalýþacak iþlev
    private void Die()
    {
        // Oyuncu ölüyor olarak iþaretlenir ve sahne yeniden yüklenir.
        Debug.Log("Player died!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
