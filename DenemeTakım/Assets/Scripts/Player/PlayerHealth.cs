using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;
    public Slider slider; // 15.02.2024 slider de�i�keni eklendi
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

        setHealth(currentHealth);

        // Sa�l�k de�erini kontrol et, �rne�in karakter �ld�yse ba�ka bir i�lem yapabilirsiniz
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
