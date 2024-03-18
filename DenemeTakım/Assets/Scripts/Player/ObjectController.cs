using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("Throw Point")]
    [SerializeField] private Transform throwableObjectPoint; // Fýrlatýlabilir nesne noktasý
    [SerializeField] private LayerMask itemLayer; // Eþya katmaný

    [Header("Pickup and Throw Variables")]
    [SerializeField] private float pickupRadius; // Eþyayý almak için kullanýlan yarýçap
    [SerializeField] private float throwForce; // Fýrlatma kuvveti

    public bool isThrowed = false;

    private GameObject currentItem; // Þu anda tutulan eþya
    private Rigidbody2D currentItemRigidbody; // Þu anda tutulan eþyanýn rigidbody bileþeni
    private Collider2D currentItemCollider; // Þu anda tutulan eþyanýn collider bileþeni
    public bool isHoldingItem = false; // Eðer bir eþya tutuluyorsa true, tutulmuyorsa false

    void Update()
    {
        // F tuþuna basýldýðýnda eþyayý al veya býrak
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isHoldingItem)
            {
                PickupItem();
            }
            else
            {
                DropItem();
            }
        }
        // Fare sað tuþu býrakýldýðýnda eþyayý fýrlat
        if (Input.GetMouseButtonUp(1) && isHoldingItem)
        {
            AimAndThrow();
        }

        // Eðer bir eþya tutuluyorsa, eþyanýn pozisyonunu güncelle
        if (isHoldingItem)
        {
            UpdateItemPosition();
        }
    }

    // Eþya al fonksiyonu
    void PickupItem()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius, itemLayer);
        if (colliders.Length > 0)
        {
            // En yakýn eþyayý seç
            float closestDistance = Mathf.Infinity;
            Collider2D closestCollider = null;

            foreach (Collider2D collider in colliders)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }

            // Seçilen eþyayý al
            if (closestCollider != null)
            {
                currentItem = closestCollider.gameObject;
                currentItem.transform.position = throwableObjectPoint.position;
                currentItemRigidbody = currentItem.GetComponent<Rigidbody2D>();
                currentItemCollider = currentItem.GetComponent<Collider2D>();
                currentItemCollider.enabled = false; // Eþyanýn collider'ýný devre dýþý býrak
                currentItemRigidbody.isKinematic = true;
                isHoldingItem = true;
            }
        }
    }

    // Eþya býrak fonksiyonu
    void DropItem()
    {
        currentItemCollider.enabled = true; // Eþyanýn collider'ýný tekrar etkinleþtir
        currentItemRigidbody.isKinematic = false;
        currentItem = null;
        currentItemRigidbody = null;
        currentItemCollider = null;
        isHoldingItem = false;
    }

    // Eþya fýrlat fonksiyonu
    void AimAndThrow()
    {
        Vector2 throwDirection = (throwableObjectPoint.position - transform.position).normalized;
        currentItemCollider.enabled = true; // Eþyanýn collider'ýný tekrar etkinleþtir
        currentItemRigidbody.isKinematic = false;

        // Fýrlatma yönüne doðru bir kuvvet uygula
        Vector2 throwForceVector = throwDirection * throwForce;
        currentItemRigidbody.AddForce(throwForceVector, ForceMode2D.Impulse);
        
        Bomb bomb = currentItem.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.StartDetonationTimer(); // Patlamayý baþlat
        }
        isThrowed = true;
        currentItem = null;
        currentItemRigidbody = null;
        currentItemCollider = null;
        isHoldingItem = false;
    }

    // Eðer bir eþya tutuluyorsa, eþyanýn pozisyonunu güncelle
    void UpdateItemPosition()
    {
        if (currentItem != null)
        {
            currentItem.transform.position = throwableObjectPoint.position;
        }
    }

    // Gizmos kullanarak alým yarýçapýný göster
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
