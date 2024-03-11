using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina;
    float staminaRegenRate = 10f; // Zamanla doldurma hýzý

    // Saðlýk çubuðunun önyüzü ve arka yüzü için Image bileþenleri
    public Image frontStaminaBar; // Deðiþtirildi: public olarak tanýmlandý
    public Image backStaminaBar; // Deðiþtirildi: public olarak tanýmlandý

    // Saðlýk çubuðunun geçiþ hýzý
    private float chipSpeed = 9f;
    private float lerptimer;
    public Color fullColor = Color.green; // Dolu renk
    public Color emptyColor = Color.red;
    void Start()
    {
       
        backStaminaBar.color = emptyColor;
        currentStamina = maxStamina;
    }

    void Update()
    {
        StaminaAutoRegen();
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }

    public void UpdateStaminaUI()
    {
        // Ön ve arka saðlýk çubuklarýnýn doluluk oranlarý alýnýr.
        float fillForeBar = frontStaminaBar.fillAmount;
        float fillBackBar = backStaminaBar.fillAmount; // Deðiþtirildi: backStaminaBar olarak düzeltildi

        // Mevcut saðlýk oraný hesaplanýr.
        float hFraction = currentStamina / maxStamina;

        // Arka saðlýk çubuðunun doluluk oraný, mevcut saðlýk oranýna eþit olana kadar yavaþça güncellenir.
        // Eðer arka saðlýk çubuðunun doluluk oraný mevcut saðlýk oranýndan büyükse, çubuðun doluluk oraný yavaþça azaltýlýr.
        if (fillBackBar > hFraction)
        {
            // Ön saðlýk çubuðu, mevcut saðlýk oranýna eþitlenene kadar yavaþça azaltýlýr.
            // Bu iþlem, saðlýk çubuklarýnýn eþ zamanlý olarak güncellenmesini saðlar.
            frontStaminaBar.fillAmount = hFraction;

            // Arka saðlýk çubuðu rengi beyaz olarak ayarlanýr.
            backStaminaBar.color = emptyColor;

            // Zamanlayýcý, doluluk oranýnýn yavaþça güncellenmesini saðlar.
            lerptimer += Time.deltaTime;

            // Zamanýn geçmesine göre, çubuðun ne kadar doldurulacaðý belirlenir.
            float percentComplete = lerptimer / chipSpeed;

            // Arka saðlýk çubuðunun doluluk oraný, mevcut doluluk oranýndan yeni doluluk oranýna doðru yumuþak bir geçiþle güncellenir.
            backStaminaBar.fillAmount = Mathf.Lerp(fillBackBar, hFraction, percentComplete);
        }

        // Ön saðlýk çubuðunun doluluk oraný, mevcut saðlýk oranýna eþit olana kadar yavaþça güncellenir.
        if (fillForeBar < hFraction)
        {
            // Arka saðlýk çubuðunun rengi mavi olarak ayarlanýr.
            // Bu, oyuncuya saðlýk çubuðunun ön tarafýnda bir deðiþiklik olduðunu gösterir.
            backStaminaBar.color = fullColor;

            // Arka saðlýk çubuðunun doluluk oraný, mevcut saðlýk oranýna eþitlenir.
            backStaminaBar.fillAmount = hFraction;

            // Zamanlayýcý, doluluk oranýnýn yavaþça güncellenmesini saðlar.
            lerptimer += Time.deltaTime;

            // Zamanýn geçmesine göre, çubuðun ne kadar doldurulacaðý belirlenir.
            float percentComplete = lerptimer / chipSpeed;

            // Ön saðlýk çubuðunun doluluk oraný, arka saðlýk çubuðunun doluluk oranýna doðru yumuþak bir geçiþle güncellenir.
            frontStaminaBar.fillAmount = Mathf.Lerp(fillForeBar, backStaminaBar.fillAmount, percentComplete);
        }
    }

    private void StaminaAutoRegen()
    {
        // Zamanla stamina'nýn yeniden doldurulmasý
        if (currentStamina < maxStamina)
        {
            lerptimer = 0f;
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // maxStamina'yi aþmamasý için kýsýtla
        }
    }

    // Public bir fonksiyon ile stamina'nýn harcanmasý
    public bool UseStamina(float amount)
    {
        lerptimer = 0f;
        if (currentStamina >= amount)
        {
           
            currentStamina -= amount;
            return true; // Stamina yeterliyse true döndür
        }
        else
        {
            return false; // Stamina yetersizse false döndür
        }
    }
}
