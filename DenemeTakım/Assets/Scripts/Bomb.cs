using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionRadius = 5f; // Patlama yarýçapý
    public float explosionForce = 10f; // Patlama kuvveti
    public float delayBeforeDetonation = 5f; // Patlamadan önceki bekleme süresi

    private bool isDetonated = false; // Patlatýldý mý?

    public void StartDetonationTimer()
    {
        Invoke("Detonate", delayBeforeDetonation);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Bomba çarptýðýnda bekletme iþlemini iptal etmek için Invoke fonksiyonunu iptal et
        CancelInvoke("Detonate");

        // Bekleme süresi sonunda patlat
        Detonate();
    }

    void Detonate()
    {
        if (!isDetonated)
        {
            // Bombanýn patlamasý için patlama konumu ve yarýçapýný belirle
            Vector3 explosionPosition = transform.position;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);

            // Her bir çarpýþma noktasýndaki nesnelere kuvvet uygula
            foreach (Collider2D hitCollider in colliders)
            {
                Rigidbody2D rb = hitCollider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Nesneye patlama kuvveti uygula
                    Vector2 direction = rb.transform.position - transform.position;
                    float distance = direction.magnitude;
                    float falloff = 1 - (distance / explosionRadius);
                    Vector2 force = direction.normalized * explosionForce * falloff;
                    rb.AddForce(force, ForceMode2D.Impulse);
                }
            }

            // Bombayý yok et
            Destroy(gameObject);
            isDetonated = true;
        }
    }

}
