using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Düþmanýn hareketini durdur
    public void StopMovement()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
    }

    // Düþmanýn hareketini yeniden baþlat
    public void RestartMovement()
    {
        rb.gravityScale = 1f;
    }
}
