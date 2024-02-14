using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public Transform throwableObjectPoint;
    public LayerMask itemLayer;
    public float pickupRadius = 2f;
    public float throwForce = 10f;

    private GameObject currentItem;
    private Rigidbody2D currentItemRigidbody;
    private Collider2D currentItemCollider;
    private bool isHoldingItem = false;

    void Update()
    {
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

        if (Input.GetMouseButtonUp(1) && isHoldingItem)
        {
            AimAndThrow();
        }

        if (isHoldingItem)
        {
            UpdateItemPosition();
        }
    }

    void PickupItem()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius, itemLayer);
        if (colliders.Length > 0)
        {
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

            if (closestCollider != null)
            {
                currentItem = closestCollider.gameObject;
                currentItem.transform.position = throwableObjectPoint.position;
                currentItemRigidbody = currentItem.GetComponent<Rigidbody2D>();
                currentItemCollider = currentItem.GetComponent<Collider2D>();
                currentItemCollider.enabled = false; // Collider'ý devre dýþý býrak
                currentItemRigidbody.isKinematic = true;
                isHoldingItem = true;
            }
        }
    }

    void DropItem()
    {
        currentItemCollider.enabled = true; // Collider'ý tekrar etkinleþtir
        currentItemRigidbody.isKinematic = false;
        currentItem = null;
        currentItemRigidbody = null;
        currentItemCollider = null;
        isHoldingItem = false;
    }

    void AimAndThrow()
    {
        Vector2 throwDirection = (throwableObjectPoint.position - transform.position).normalized;
        currentItemCollider.enabled = true; // Collider'ý tekrar etkinleþtir
        currentItemRigidbody.isKinematic = false;
        currentItemRigidbody.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
        currentItem = null;
        currentItemRigidbody = null;
        currentItemCollider = null;
        isHoldingItem = false;
    }

    void UpdateItemPosition()
    {
        if (currentItem != null)
        {
            currentItem.transform.position = throwableObjectPoint.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
