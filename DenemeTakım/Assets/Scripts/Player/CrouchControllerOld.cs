using UnityEngine;

public class dController : MonoBehaviour
{
    private CapsuleCollider2D capsusleCollider;

    private Vector2 originalColliderSize;
    private Vector3 originalScale;
    
    private bool isCrouching = false;

    private void Start()
    {
        capsusleCollider = GetComponent<CapsuleCollider2D>();
        originalColliderSize = capsusleCollider.size;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCrouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y/2, transform.localScale.z);
                capsusleCollider.size = new Vector2(capsusleCollider.size.x, capsusleCollider.size.y / 2f);
                isCrouching = true;
            }
            else
            {
                transform.localScale = originalScale;
                capsusleCollider.size = originalColliderSize;
                isCrouching = false;
            }
        }
    }
}