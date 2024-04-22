using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    public LayerMask destroyLayer; // Mermiyi yok etmek istedi�imiz layer

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // �arp�lan obje oyuncu ise hasar uygula
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10f);
            }
        }

        // �arp�lan objenin layer'�n� kontrol et
        if (destroyLayer == (destroyLayer | (1 << collision.gameObject.layer)))
        {
            // E�er �arp�lan obje destroyLayer'a ait bir layer ise, mermiyi yok et
            Destroy(gameObject);
        }
    }
}
