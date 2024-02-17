using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Knockback : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 10f; // Geri tepme kuvveti
    [SerializeField] private float knockbackDuration = 0.2f; // Geri tepme s�resi
    public Vector2 knockbackDirection; // Geri tepme y�n�

    // D��mana geri tepme uygula
    public void ApplyKnockback()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DoKnockback(rb));
   
    }

    // Geri tepme i�lemini y�r�t
    private IEnumerator DoKnockback(Rigidbody2D rb)
    {
        // Geri tepme kuvveti ve y�n�ne g�re h�z� ayarla
        Vector2 knockbackVelocity = knockbackDirection.normalized * knockbackForce;
        rb.velocity = knockbackVelocity;

        // Geri tepme s�resi boyunca hareket etme
        float timer = 0;
        while (timer < knockbackDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Geri tepme s�resi bitti�inde h�z� s�f�rla
        rb.velocity = Vector2.zero;
    }
}
