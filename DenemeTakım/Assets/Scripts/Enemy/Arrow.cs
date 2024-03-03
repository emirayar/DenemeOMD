using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int arrowDamage = 10; // Okun verdi�i hasar

    private Rigidbody2D rb;
    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        // Rigidbody bile�enini al
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Karaktere �arp�ld���nda
        if (collision.CompareTag("Player"))
        {
            // Karaktere hasar ver
            collision.GetComponent<PlayerHealth>().TakeDamage(arrowDamage);
            // Oku yok et
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        // Ok yere �arpt�ysa ve hala hareket ediyorsa, hareketi durdur
        if (rb.IsTouchingLayers(layerMask))
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.isKinematic = true;
            Destroy(gameObject);
        }
    }
}
