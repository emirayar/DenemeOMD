using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    public LayerMask destroyLayer; // Mermiyi yok etmek istediðimiz layer

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Çarpýlan obje oyuncu ise hasar uygula
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10f);
            }
        }

        // Çarpýlan objenin layer'ýný kontrol et
        if (destroyLayer == (destroyLayer | (1 << collision.gameObject.layer)))
        {
            // Eðer çarpýlan obje destroyLayer'a ait bir layer ise, mermiyi yok et
            Destroy(gameObject);
        }
    }
}
