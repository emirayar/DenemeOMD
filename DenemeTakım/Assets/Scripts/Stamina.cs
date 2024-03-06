using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public float maxStamina;
    public float currentStamina;
    [SerializeField] float staminaRegenRate;// Zamanla doldurma h�z�

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        StaminaAutoRegen();
    }

    private void StaminaAutoRegen()
    {
        // Zamanla stamina'n�n yeniden doldurulmas�
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // maxStamina'yi a�mamas� i�in k�s�tla
        }
    }
    // Public bir fonksiyon ile stamina'n�n harcanmas�
    public bool UseStamina(float amount)
    {
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
