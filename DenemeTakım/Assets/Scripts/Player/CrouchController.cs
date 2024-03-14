using UnityEngine;

public class CrouchController : MonoBehaviour
{
    public bool isCrouching = false; // Çömelme durumu (Diðer Scriptlerde kullanmak için)
    private CapsuleCollider2D capsuleCollider; // Karakter kapsül collider'ý deðiþkeni
    private float initialColliderSize; // Ayakta durma durumundaki collider boyutu deðiþkeni
    private float initialScaleValue; // Ayakta durma durumundaki scale deðeri deðiþkeni
    private PlayerMovement playerMovement; // PlayerMovement scripti deðiþkeni
    private JumpController jumpController; // JumpController scripti deðiþkeni

    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerMovement = GetComponent<PlayerMovement>();
        jumpController = GetComponent<JumpController>();

        initialColliderSize = capsuleCollider.size.y;
        initialScaleValue = transform.localScale.y;
    }

    private void Update()
    {
        Crouch();
    }
    private void Crouch()
    {
        // Çömelme tuþuna basýldýðýnda
        if (Input.GetKey(KeyCode.C))
        {
            isCrouching = true;
            playerMovement.moveSpeed = 2f;
            jumpController.jumpForceMin = 0f;
            jumpController.jumpForceMax = 0f;
            // Collider'ýn yüksekliðini ve lokal scale'ý çömelme durumuna göre ayarla
            transform.localScale = new Vector3(transform.localScale.x, initialScaleValue / 2, transform.localScale.z);
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, initialColliderSize / 2);
        }
        else // Çömelme durumunda deðilse
        {
            isCrouching = false;
            playerMovement.moveSpeed = 5f;
            jumpController.jumpForceMin = 1f;
            jumpController.jumpForceMax = 5f;
            // Collider'ýn boyutunu ve lokal scale'ý baþlangýç deðerlerine geri yükle
            transform.localScale = new Vector3(transform.localScale.x, initialScaleValue, transform.localScale.z);
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, initialColliderSize);
        }
    }
}
