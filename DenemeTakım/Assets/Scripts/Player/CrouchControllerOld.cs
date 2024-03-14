using UnityEngine;

public class dController : MonoBehaviour
{
    private Vector2 originalColliderSize;

    private void Start()
    {
        originalColliderSize = GetComponent<CapsuleCollider2D>().size;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Vector3 scale = transform.localScale;
            scale.y /= 2f;
            transform.localScale = scale;

            CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
            collider.size = new Vector2(collider.size.x, originalColliderSize.y / 2f);
        }
    }
}