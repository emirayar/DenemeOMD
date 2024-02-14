using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Slider slider; // 15.02.2024 slider deðiþkeni eklendi
    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void setHealth(int currentHealth)

    {
        slider.value = currentHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Saðlýk deðerini kontrol et, örneðin karakter öldüyse baþka bir iþlem yapabilirsiniz
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
