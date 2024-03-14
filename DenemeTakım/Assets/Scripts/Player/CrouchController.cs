using UnityEngine;

public class CrouchController : MonoBehaviour
{
    public bool isCrouching = false; // ��melme durumu (Di�er Scriptlerde kullanmak i�in)
    private CapsuleCollider2D capsuleCollider; // Karakter kaps�l collider'� de�i�keni
    private float initialColliderSize; // Ayakta durma durumundaki collider boyutu de�i�keni
    private float initialScaleValue; // Ayakta durma durumundaki scale de�eri de�i�keni
    private PlayerMovement playerMovement; // PlayerMovement scripti de�i�keni
    private JumpController jumpController; // JumpController scripti de�i�keni

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
        // ��melme tu�una bas�ld���nda
        if (Input.GetKey(KeyCode.C))
        {
            isCrouching = true;
            playerMovement.moveSpeed = 2f;
            jumpController.jumpForceMin = 0f;
            jumpController.jumpForceMax = 0f;
            // Collider'�n y�ksekli�ini ve lokal scale'� ��melme durumuna g�re ayarla
            transform.localScale = new Vector3(transform.localScale.x, initialScaleValue / 2, transform.localScale.z);
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, initialColliderSize / 2);
        }
        else // ��melme durumunda de�ilse
        {
            isCrouching = false;
            playerMovement.moveSpeed = 5f;
            jumpController.jumpForceMin = 1f;
            jumpController.jumpForceMax = 5f;
            // Collider'�n boyutunu ve lokal scale'� ba�lang�� de�erlerine geri y�kle
            transform.localScale = new Vector3(transform.localScale.x, initialScaleValue, transform.localScale.z);
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, initialColliderSize);
        }
    }
}
