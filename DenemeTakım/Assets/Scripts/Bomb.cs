using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float maxExplosionForce = 25f; // Maksimum patlama kuvveti
    [SerializeField] private float maxDamageDistance = 5f; // Maksimum hasar mesafesi
    [SerializeField] private float detonationTime = 5f; // Patlama zamanlay�c�s�

    private bool isDetonating = false; // Patlama s�recinde mi?

    private ObjectController objectController;

    private void Start()
    {
        objectController = FindObjectOfType<ObjectController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (objectController.isThrowed)
        {
            // Bomba bir nesneye �arpt���nda patlamay� ba�lat
            if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy")))
            {
                Detonate();
            }
        }
    }


    public void StartDetonationTimer()
    {
        if (!isDetonating)
        {
            isDetonating = true;
            Invoke("Detonate", detonationTime); // Belirtilen s�re sonunda Detonate fonksiyonunu �a��r
        }
    }

    private void Detonate()
    {
        Collider2D[] objectsInExplosionRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D obj in objectsInExplosionRadius)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();
            Health health = obj.GetComponent<Health>();

            if (rb != null)
            {
                ApplyExplosionForceAndDamage(rb, obj.transform.position - transform.position, playerHealth, health);
            }
        }

        Destroy(gameObject);
    }

    private void ApplyExplosionForceAndDamage(Rigidbody2D rb, Vector2 direction, PlayerHealth playerHealth, Health health)
    {
        float distance = direction.magnitude;
        if (distance > 0)
        {
            // Hasar mesafesini belirle ve hasar� hesapla
            float damageDistanceRatio = Mathf.Clamp01(distance / maxDamageDistance);
            int damage = Mathf.RoundToInt(damageDistanceRatio * (playerHealth != null ? playerHealth.maxHealth : health.maxHealth));

            // Patlama kuvvetini do�ru y�nde ve maksimum s�n�rda uygula
            float explosionForce = Mathf.Min(maxExplosionForce, maxExplosionForce * damageDistanceRatio);
            rb.AddForce(direction.normalized * explosionForce, ForceMode2D.Impulse);

            // Hasar� uygula
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
            else if (health != null)
                health.TakeDamage(damage);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
