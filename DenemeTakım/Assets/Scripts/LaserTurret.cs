using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    public Transform firePoint; 
    public LayerMask playerMask; 
    public LayerMask groundMask; 
    public float detectionRadius; 
    public float fireRate; 
    public GameObject bulletPrefab; 
    public float bulletSpeed; 

    private GameObject target; // hedef obje
    private float fireTimer;

    void Start()
    {
        fireTimer = fireRate;
        DetectTarget();
    }

    void Update()
    {
        if (target != null && IsTargetInRange())
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Fire();
                fireTimer = fireRate;
            }
        }
        else
        {
            DetectTarget();
        }
    }

    bool IsTargetInRange()
    {
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            return distanceToTarget <= detectionRadius;
        }
            return false;
    }


    void DetectTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (Collider2D collider in colliders)
        {
            Vector2 direction = collider.transform.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRadius, groundMask);
            if (hit.collider == null)
            {
                // Engeller yoksa ve oyuncu g�r�l�yorsa hedef olarak belirle
                RaycastHit2D playerHit = Physics2D.Raycast(transform.position, direction, detectionRadius, playerMask);
                if (playerHit.collider != null && (playerHit.collider.CompareTag("Player")))
                {
                    target = playerHit.collider.gameObject;
                    return;
                }
            }
        }

        target = null;
    }


    void Fire()
    {
        Vector2 groundDirection = transform.position - target.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, groundDirection, detectionRadius, groundMask);
        if (hit.collider != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = (target.transform.position - firePoint.position).normalized;
            bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
