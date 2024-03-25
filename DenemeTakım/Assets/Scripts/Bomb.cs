using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForceMultiplier;
    [SerializeField] private float detonationTime;
    
    [SerializeField] private float distance;



    private bool isDetonating = false;

    private ObjectController objectController;

    private void Start()
    {
        objectController = FindObjectOfType<ObjectController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (objectController.isThrowed)
        {
            // Bomba bir nesneye çarptýðýnda patlamayý baþlat
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
            Invoke("Detonate", detonationTime); // Belirtilen süre sonunda Detonate fonksiyonunu çaðýr
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
        distance = direction.magnitude;

        if (distance > 0)
        {
            float explosionForce = explosionForceMultiplier / distance ;

            rb.AddForce(direction.normalized * explosionForce);
        }

        // Apply damage if the corresponding health component is available
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(100 / direction.magnitude);
            Debug.Log(100 / direction.magnitude);
        }else if (health != null)
        {
            health.TakeDamage(100 / direction.magnitude);
            Debug.Log(100 / direction.magnitude);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
