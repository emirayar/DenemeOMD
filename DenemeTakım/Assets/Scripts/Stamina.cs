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

    // Sa�l�k �ubu�unun ge�i� h�z�
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
        // �n ve arka sa�l�k �ubuklar�n�n doluluk oranlar� al�n�r.
        float fillForeBar = frontStaminaBar.fillAmount;
        float fillBackBar = backStaminaBar.fillAmount; // De�i�tirildi: backStaminaBar olarak d�zeltildi

        // Mevcut sa�l�k oran� hesaplan�r.
        float hFraction = currentStamina / maxStamina;

        // Arka sa�l�k �ubu�unun doluluk oran�, mevcut sa�l�k oran�na e�it olana kadar yava��a g�ncellenir.
        // E�er arka sa�l�k �ubu�unun doluluk oran� mevcut sa�l�k oran�ndan b�y�kse, �ubu�un doluluk oran� yava��a azalt�l�r.
        if (fillBackBar > hFraction)
        {
            // �n sa�l�k �ubu�u, mevcut sa�l�k oran�na e�itlenene kadar yava��a azalt�l�r.
            // Bu i�lem, sa�l�k �ubuklar�n�n e� zamanl� olarak g�ncellenmesini sa�lar.
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
            // Bu, oyuncuya sa�l�k �ubu�unun �n taraf�nda bir de�i�iklik oldu�unu g�sterir.
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
}
