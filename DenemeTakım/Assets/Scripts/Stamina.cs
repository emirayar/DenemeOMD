using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public float maxStamina;
    public float currentStamina;
    [SerializeField] float staminaRegenRate;// Zamanla doldurma hýzý

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
        // Zamanla stamina'nýn yeniden doldurulmasý
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // maxStamina'yi aþmamasý için kýsýtla
        }
    }
    // Public bir fonksiyon ile stamina'nýn harcanmasý
    public bool UseStamina(float amount)
    {
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
