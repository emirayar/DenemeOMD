using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;       

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    public float chipSpeed = 150f;
    private float lerptimer;
    public Image frontHealthBar;
    public Image BackHealthBar;
    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth,0,maxHealth);
        UpdateHealthUI();
        if (Input.GetKeyDown(KeyCode.H))
        {
            RestoreHealth(10f);

        }
    }
    public void UpdateHealthUI()
    {
        Debug.Log(currentHealth);
        float fillF = frontHealthBar.fillAmount;
        float fillB = BackHealthBar.fillAmount;
        float hFraction = currentHealth / maxHealth;

        if (fillB > hFraction) 
        {
         frontHealthBar.fillAmount = hFraction;
            BackHealthBar.color = Color.white;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackHealthBar.fillAmount = Mathf.Lerp(fillB,hFraction, percentComplete);


        }
        if (fillF < hFraction) 
        {
            BackHealthBar.color = Color.blue;
            BackHealthBar.fillAmount = hFraction;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, BackHealthBar.fillAmount, percentComplete);
        }

    }
    public void setHealth(float currentHealth)
    {
        
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        lerptimer = 0f;
        setHealth(currentHealth);

        // Saðlýk deðerini kontrol et, örneðin karakter öldüyse baþka bir iþlem yapabilirsiniz
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void RestoreHealth (float healAmount)
    {
     currentHealth += healAmount;
        lerptimer = 0f;
       
    }

    private void Die()
    {
        Debug.Log("Player died!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
