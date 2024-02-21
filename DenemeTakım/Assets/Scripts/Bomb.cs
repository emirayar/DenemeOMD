using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionRadius = 5f; // Patlama yar��ap�
    public float explosionForce = 10f; // Patlama kuvveti
    public float delayBeforeDetonation = 5f; // Patlamadan �nceki bekleme s�resi

    private bool isDetonated = false; // Patlat�ld� m�?

    public void StartDetonationTimer()
    {
        Invoke("Detonate", delayBeforeDetonation);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Bomba �arpt���nda bekletme i�lemini iptal etmek i�in Invoke fonksiyonunu iptal et
        CancelInvoke("Detonate");

        // Bekleme s�resi sonunda patlat
        Detonate();
    }

    void Detonate()
    {
        if (!isDetonated)
        {
            // Bomban�n patlamas� i�in patlama konumu ve yar��ap�n� belirle
            Vector3 explosionPosition = transform.position;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);

            // Her bir �arp��ma noktas�ndaki nesnelere kuvvet uygula
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

            // Bombay� yok et
            Destroy(gameObject);
            isDetonated = true;
        }
    }

}
