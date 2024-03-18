using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Variables")]
    // Maksimum sa�l�k puan�
    public float maxHealth = 100f;

    // Oyuncunun mevcut sa�l�k puan�
    private float currentHealth;

    [Header("Chip Variables")]
    // Sa�l�k �ubu�unun ge�i� h�z�
    [SerializeField] private float chipSpeed = 150f;

    //Lineer interpolasyon kullanarak �ubu�un yava� yava� hareket etmesi i�in kulland���m�z timer
    private float lerptimer;

    private CinemachineImpulseSource impulseSource;

    [Header("Health Bars")]
    // Sa�l�k �ubu�unun �ny�z� ve arka y�z� i�in Image bile�enleri
    [SerializeField] private Image frontHealthBar;
    [SerializeField] private Image BackHealthBar;

    // Ba�lang��ta oyuncunun maksimum sa�l�k de�erine sahip olmas�n� sa�lar.
    private void Start()
    {
        currentHealth = maxHealth;
        impulseSource = GetComponentInChildren<CinemachineImpulseSource> ();
    }

    // Her g�ncellemede sa�l�k durumunu kontrol eder ve UI'yi g�nceller.
    private void Update()
    {
        // Sa�l�k de�erini s�n�rlar
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // UI'yi g�nceller
        UpdateHealthUI();

        // H tu�una bas�ld���nda, sa�l�k de�erini art�r�r.
        if (Input.GetKeyDown(KeyCode.H))
        {
            RestoreHealth(10f);
        }
    }

    // Sa�l�k UI's�n� g�nceller
    public void UpdateHealthUI()
    {
        // �n ve arka sa�l�k �ubuklar�n�n doluluk oranlar� al�n�r.
        float fillForeBar = frontHealthBar.fillAmount;
        float fillBackBar = BackHealthBar.fillAmount;

        // Mevcut sa�l�k oran� hesaplan�r.
        float hFraction = currentHealth / maxHealth;

        // Arka sa�l�k �ubu�unun doluluk oran�, mevcut sa�l�k oran�na e�it olana kadar yava��a g�ncellenir.
        // E�er arka sa�l�k �ubu�unun doluluk oran� mevcut sa�l�k oran�ndan b�y�kse, �ubu�un doluluk oran� yava��a azalt�l�r.
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

        // �n sa�l�k �ubu�unun doluluk oran�, mevcut sa�l�k oran�na e�it olana kadar yava��a g�ncellenir.
        if (fillForeBar < hFraction)
        {
            // Arka sa�l�k �ubu�unun rengi mavi olarak ayarlan�r.
            // Bu, oyuncuya sa�l�k �ubu�unun �n taraf�nda bir de�i�iklik oldu�unu g�sterir.
            BackHealthBar.color = Color.green;

            // Arka sa�l�k �ubu�unun doluluk oran�, mevcut sa�l�k oran�na e�itlenir.
            BackHealthBar.fillAmount = hFraction;

            // Zamanlay�c�, doluluk oran�n�n yava��a g�ncellenmesini sa�lar.
            lerptimer += Time.deltaTime;

            // Zaman�n ge�mesine g�re, �ubu�un ne kadar doldurulaca�� belirlenir.
            float percentComplete = lerptimer / chipSpeed;

            // �n sa�l�k �ubu�unun doluluk oran�, arka sa�l�k �ubu�unun doluluk oran�na do�ru yumu�ak bir ge�i�le g�ncellenir.
            frontHealthBar.fillAmount = Mathf.Lerp(fillForeBar, BackHealthBar.fillAmount, percentComplete);
        }

    }

    // Oyuncunun hasar almas�n� sa�lar
    public void TakeDamage(int damage)
    {
        // Mevcut sa�l�k de�eri hasar kadar azalt�l�r.
        currentHealth -= damage;
        lerptimer = 0f;
        impulseSource.GenerateImpulse();

        // Sa�l�k s�f�rlan�rsa, �l�m i�levi �a��r�l�r.
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Oyuncunun sa�l���n� art�r�r
    public void RestoreHealth(float healAmount)
    {
        // Mevcut sa�l�k de�eri art�r�l�r.
        currentHealth += healAmount;
        lerptimer = 0f;
    }

    // Oyuncunun �l�m� durumunda �al��acak i�lev
    private void Die()
    {
        // Oyuncu �l�yor olarak i�aretlenir ve sahne yeniden y�klenir.
        Debug.Log("Player died!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
