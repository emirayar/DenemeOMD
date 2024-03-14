using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina;
    float staminaRegenRate = 10f; // Zamanla doldurma h�z�

    // Sa�l�k �ubu�unun �ny�z� ve arka y�z� i�in Image bile�enleri
    public Image frontStaminaBar; // De�i�tirildi: public olarak tan�mland�
    public Image backStaminaBar; // De�i�tirildi: public olarak tan�mland�
    public Image decreasingEffect;
    // Sa�l�k �ubu�unun ge�i� h�z�
    private float chipSpeed = 9f;
    private float lerptimer;
    public Color fullColor = Color.green; // Dolu renk
    public Color emptyColor = Color.red;

    private bool decreasingEffectActive = false;
    private float decreasingEffectDuration = 0.2f;

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
        // �n ve arka sa�l�k �ubuklar�n�n doluluk oranlar� al�n�r.
        float fillForeBar = frontStaminaBar.fillAmount;
        float fillBackBar = backStaminaBar.fillAmount;

        // Mevcut sa�l�k oran� hesaplan�r.
        float hFraction = currentStamina / maxStamina;

        // Arka sa�l�k �ubu�unun doluluk oran�, mevcut sa�l�k oran�na e�it olana kadar yava��a g�ncellenir.
        if (fillBackBar > hFraction)
        {
            // Arka sa�l�k �ubu�u, mevcut sa�l�k oran�na e�itlenene kadar yava��a azalt�l�r.
            frontStaminaBar.fillAmount = hFraction;

            // Arka sa�l�k �ubu�u rengi beyaz olarak ayarlan�r.
            backStaminaBar.color = emptyColor;

            // Zamanlay�c�, doluluk oran�n�n yava��a g�ncellenmesini sa�lar.
            lerptimer += Time.deltaTime;

            // Zaman�n ge�mesine g�re, �ubu�un ne kadar doldurulaca�� belirlenir.
            float percentComplete = lerptimer / chipSpeed;

            // Arka sa�l�k �ubu�unun doluluk oran�, mevcut doluluk oran�ndan yeni doluluk oran�na do�ru yumu�ak bir ge�i�le g�ncellenir.
            backStaminaBar.fillAmount = Mathf.Lerp(fillBackBar, hFraction, percentComplete);
        }

        // �n sa�l�k �ubu�unun doluluk oran�, mevcut sa�l�k oran�na e�it olana kadar yava��a g�ncellenir.
        if (fillForeBar < hFraction)
        {
            // Arka sa�l�k �ubu�unun rengi mavi olarak ayarlan�r.
            backStaminaBar.color = fullColor;

            // Arka sa�l�k �ubu�unun doluluk oran�, mevcut sa�l�k oran�na e�itlenir.
            backStaminaBar.fillAmount = hFraction;

            // Zamanlay�c�, doluluk oran�n�n yava��a g�ncellenmesini sa�lar.
            lerptimer += Time.deltaTime;

            // Zaman�n ge�mesine g�re, �ubu�un ne kadar doldurulaca�� belirlenir.
            float percentComplete = lerptimer / chipSpeed;

            // �n sa�l�k �ubu�unun doluluk oran�, arka sa�l�k �ubu�unun doluluk oran�na do�ru yumu�ak bir ge�i�le g�ncellenir.
            frontStaminaBar.fillAmount = Mathf.Lerp(fillForeBar, backStaminaBar.fillAmount, percentComplete);
        }
    }

    private void StaminaAutoRegen()
    {
        // Zamanla stamina'n�n yeniden doldurulmas�
        if (currentStamina < maxStamina)
        {
            lerptimer = 0f;
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // maxStamina'yi a�mamas� i�in k�s�tla
        }
    }

    // Public bir fonksiyon ile stamina'n�n harcanmas�
    public bool UseStamina(float amount)
    {
        lerptimer = 0f;
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true; // Stamina yeterliyse true d�nd�r
        }
        else
        {
            return false; // Stamina yetersizse false d�nd�r
        }
    }

    public void DecreasingEffect()
    {
        if (!decreasingEffectActive)
        { 
            StartCoroutine(StartDecreasingEffect()); 
        }
    }

    IEnumerator StartDecreasingEffect()
    {
        decreasingEffectActive = true;
        float elapsedTime = 0f;
        Color initialColor = decreasingEffect.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0.5f); // Hedef rengi belirle

        while (elapsedTime < decreasingEffectDuration)
        {
            elapsedTime += Time.deltaTime;

            // Alpha de�eri zamanla artar, ard�ndan d��er
            decreasingEffect.color = Color.Lerp(initialColor, targetColor, elapsedTime / decreasingEffectDuration);

            yield return null;
        }

        decreasingEffect.color = targetColor; // Alpha de�erini hedef al�nan de�ere ayarlay�n

        yield return new WaitForSeconds(0.2f); // Bekleme s�resi

        elapsedTime = 0f;
        while (elapsedTime < decreasingEffectDuration)
        {
            elapsedTime += Time.deltaTime;

            // Alpha de�eri zamanla azal�r, ard�ndan artar
            decreasingEffect.color = Color.Lerp(targetColor, initialColor, elapsedTime / decreasingEffectDuration);

            yield return null;
        }

        decreasingEffect.color = initialColor; // Alfa de�eri ba�lang�� rengine ayarlay�n
        decreasingEffectActive = false;
    }
}
