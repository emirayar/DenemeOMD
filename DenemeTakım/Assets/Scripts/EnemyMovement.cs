using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // D��man�n hareketini durdur
    public void StopMovement()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
    }

    // D��man�n hareketini yeniden ba�lat
    public void RestartMovement()
    {
        rb.gravityScale = 1f;
    }
}
