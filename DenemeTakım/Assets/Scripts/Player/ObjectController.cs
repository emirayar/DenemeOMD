using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField] private Transform throwableObjectPoint; // F�rlat�labilir nesne noktas�
    [SerializeField] private LayerMask itemLayer; // E�ya katman�
    [SerializeField] private float pickupRadius = 2f; // E�yay� almak i�in kullan�lan yar��ap
    [SerializeField] private float throwForce = 10f; // F�rlatma kuvveti

    [SerializeField] private int numPoints = 30; // �izgi i�in kullan�lacak nokta say�s�
    [SerializeField] private float lineWidth = 0.1f; // �izgi kal�nl���

    private LineRenderer lineRenderer; // �izgi olu�turmak i�in kullan�lan bile�en

    private GameObject currentItem; // �u anda tutulan e�ya
    private Rigidbody2D currentItemRigidbody; // �u anda tutulan e�yan�n rigidbody bile�eni
    private Collider2D currentItemCollider; // �u anda tutulan e�yan�n collider bile�eni
    private bool isHoldingItem = false; // E�er bir e�ya tutuluyorsa true, tutulmuyorsa false

    void Start()
    {
        // Line Renderer bile�enini al
        lineRenderer = GetComponent<LineRenderer>();

        // �izgi kal�nl���n� ayarla
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    void Update()
    {
        // F tu�una bas�ld���nda e�yay� al veya b�rak
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

        // Fare sa� tu�u bas�l� tutuldu�unda �izgiyi olu�tur
        if (Input.GetMouseButton(1) && isHoldingItem)
        {
            AimTrajectory();
        }

        // Fare sa� tu�u b�rak�ld���nda e�yay� f�rlat
        if (Input.GetMouseButtonUp(1) && isHoldingItem)
        {
            AimAndThrow();
            lineRenderer.positionCount = 0; // �izgiyi s�f�rla
        }

        // E�er bir e�ya tutuluyorsa, e�yan�n pozisyonunu g�ncelle
        if (isHoldingItem)
        {
            UpdateItemPosition();
        }
    }

    // E�ya al fonksiyonu
    void PickupItem()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius, itemLayer);
        if (colliders.Length > 0)
        {
            // En yak�n e�yay� se�
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

            // Se�ilen e�yay� al
            if (closestCollider != null)
            {
                currentItem = closestCollider.gameObject;
                currentItem.transform.position = throwableObjectPoint.position;
                currentItemRigidbody = currentItem.GetComponent<Rigidbody2D>();
                currentItemCollider = currentItem.GetComponent<Collider2D>();
                currentItemCollider.enabled = false; // E�yan�n collider'�n� devre d��� b�rak
                currentItemRigidbody.isKinematic = true;
                isHoldingItem = true;
            }
        }
    }

    // E�ya b�rak fonksiyonu
    void DropItem()
    {
        currentItemCollider.enabled = true; // E�yan�n collider'�n� tekrar etkinle�tir
        currentItemRigidbody.isKinematic = false;
        currentItem = null;
        currentItemRigidbody = null;
        currentItemCollider = null;
        isHoldingItem = false;
    }

    // E�ya f�rlat fonksiyonu
    void AimAndThrow()
    {
        Vector2 throwDirection = (throwableObjectPoint.position - transform.position).normalized;
        currentItemCollider.enabled = true; // E�yan�n collider'�n� tekrar etkinle�tir
        currentItemRigidbody.isKinematic = false;

        // F�rlatma y�n�ne do�ru bir kuvvet uygula
        Vector2 throwForceVector = throwDirection * throwForce;
        currentItemRigidbody.AddForce(throwForceVector, ForceMode2D.Impulse);

        currentItem = null;
        currentItemRigidbody = null;
        currentItemCollider = null;
        isHoldingItem = false;
    }

    // �izgiyi olu�tur fonksiyonu
    void AimTrajectory()
    {
        float mass = 0.06f; // Cismin k�tlesi
        float throwForce = 1f; // F�rlatma kuvveti (throwForce)

        // Cismin h�z�n� hesapla
        Vector2 throwDirection = (throwableObjectPoint.position - transform.position).normalized;
        Vector2 throwVelocity = throwDirection * throwForce / mass;

        // Zaman ad�m�n� ayarla
        float timeStep = 0.05f; // Zaman ad�m�n� ayarlayal�m (�rne�in)

        // �izgiyi olu�tururken her bir noktan�n pozisyonunu hesapla
        Vector2 currentPosition = throwableObjectPoint.position;
        Vector2 currentVelocity = throwVelocity;

        lineRenderer.positionCount = numPoints; // �izgi i�in nokta say�s�n� ayarla

        for (int i = 0; i < numPoints; i++)
        {
            currentPosition += currentVelocity * timeStep;
            currentVelocity += Physics2D.gravity * timeStep;
            lineRenderer.SetPosition(i, currentPosition);
        }
    }

    // E�er bir e�ya tutuluyorsa, e�yan�n pozisyonunu g�ncelle
    void UpdateItemPosition()
    {
        if (currentItem != null)
        {
            currentItem.transform.position = throwableObjectPoint.position;
        }
    }

    // Gizmos kullanarak al�m yar��ap�n� g�ster
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
