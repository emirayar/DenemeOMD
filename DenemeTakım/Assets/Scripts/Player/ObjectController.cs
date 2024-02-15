using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField] private Transform throwableObjectPoint; // Fýrlatýlabilir nesne noktasý
    [SerializeField] private LayerMask itemLayer; // Eþya katmaný
    [SerializeField] private float pickupRadius = 2f; // Eþyayý almak için kullanýlan yarýçap
    [SerializeField] private float throwForce = 10f; // Fýrlatma kuvveti

    [SerializeField] private int numPoints = 30; // Çizgi için kullanýlacak nokta sayýsý
    [SerializeField] private float lineWidth = 0.1f; // Çizgi kalýnlýðý

    private LineRenderer lineRenderer; // Çizgi oluþturmak için kullanýlan bileþen

    private GameObject currentItem; // Þu anda tutulan eþya
    private Rigidbody2D currentItemRigidbody; // Þu anda tutulan eþyanýn rigidbody bileþeni
    private Collider2D currentItemCollider; // Þu anda tutulan eþyanýn collider bileþeni
    private bool isHoldingItem = false; // Eðer bir eþya tutuluyorsa true, tutulmuyorsa false

    void Start()
    {
        // Line Renderer bileþenini al
        lineRenderer = GetComponent<LineRenderer>();

        // Çizgi kalýnlýðýný ayarla
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

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

        // Fare sað tuþu basýlý tutulduðunda çizgiyi oluþtur
        if (Input.GetMouseButton(1) && isHoldingItem)
        {
            AimTrajectory();
        }

        // Fare sað tuþu býrakýldýðýnda eþyayý fýrlat
        if (Input.GetMouseButtonUp(1) && isHoldingItem)
        {
            AimAndThrow();
            lineRenderer.positionCount = 0; // Çizgiyi sýfýrla
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

        currentItem = null;
        currentItemRigidbody = null;
        currentItemCollider = null;
        isHoldingItem = false;
    }

    // Çizgiyi oluþtur fonksiyonu
    void AimTrajectory()
    {
        float mass = 0.06f; // Cismin kütlesi
        float throwForce = 1f; // Fýrlatma kuvveti (throwForce)

        // Cismin hýzýný hesapla
        Vector2 throwDirection = (throwableObjectPoint.position - transform.position).normalized;
        Vector2 throwVelocity = throwDirection * throwForce / mass;

        // Zaman adýmýný ayarla
        float timeStep = 0.05f; // Zaman adýmýný ayarlayalým (örneðin)

        // Çizgiyi oluþtururken her bir noktanýn pozisyonunu hesapla
        Vector2 currentPosition = throwableObjectPoint.position;
        Vector2 currentVelocity = throwVelocity;

        lineRenderer.positionCount = numPoints; // Çizgi için nokta sayýsýný ayarla

        for (int i = 0; i < numPoints; i++)
        {
            currentPosition += currentVelocity * timeStep;
            currentVelocity += Physics2D.gravity * timeStep;
            lineRenderer.SetPosition(i, currentPosition);
        }
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
