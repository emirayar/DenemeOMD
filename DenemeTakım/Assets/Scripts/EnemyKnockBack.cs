using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Knockback : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 10f; // Geri tepme kuvveti
    [SerializeField] private float knockbackDuration = 0.2f; // Geri tepme süresi
    public Vector2 knockbackDirection; // Geri tepme yönü

    // Düþmana geri tepme uygula
    public void ApplyKnockback()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DoKnockback(rb));
   
    }

    // Geri tepme iþlemini yürüt
    private IEnumerator DoKnockback(Rigidbody2D rb)
    {
        // Geri tepme kuvveti ve yönüne göre hýzý ayarla
        Vector2 knockbackVelocity = knockbackDirection.normalized * knockbackForce;
        rb.velocity = knockbackVelocity;

        // Geri tepme süresi boyunca hareket etme
        float timer = 0;
        while (timer < knockbackDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Geri tepme süresi bittiðinde hýzý sýfýrla
        rb.velocity = Vector2.zero;
    }
}
