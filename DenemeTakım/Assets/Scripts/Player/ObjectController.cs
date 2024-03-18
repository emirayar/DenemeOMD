using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("Throw Point")]
    [SerializeField] private Transform throwableObjectPoint; // F�rlat�labilir nesne noktas�
    [SerializeField] private LayerMask itemLayer; // E�ya katman�

    [Header("Pickup and Throw Variables")]
    [SerializeField] private float pickupRadius; // E�yay� almak i�in kullan�lan yar��ap
    [SerializeField] private float throwForce; // F�rlatma kuvveti

    public bool isThrowed = false;

    private GameObject currentItem; // �u anda tutulan e�ya
    private Rigidbody2D currentItemRigidbody; // �u anda tutulan e�yan�n rigidbody bile�eni
    private Collider2D currentItemCollider; // �u anda tutulan e�yan�n collider bile�eni
    public bool isHoldingItem = false; // E�er bir e�ya tutuluyorsa true, tutulmuyorsa false

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
        // Fare sa� tu�u b�rak�ld���nda e�yay� f�rlat
        if (Input.GetMouseButtonUp(1) && isHoldingItem)
        {
            AimAndThrow();
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
        
        Bomb bomb = currentItem.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.StartDetonationTimer(); // Patlamay� ba�lat
        }
        isThrowed = true;
        currentItem = null;
        currentItemRigidbody = null;
        currentItemCollider = null;
        isHoldingItem = false;
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
