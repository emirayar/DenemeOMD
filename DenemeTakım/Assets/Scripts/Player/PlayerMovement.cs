using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

// Oyuncu karakterinin hareketini kontrol eden C# script'i
public class PlayerMovement : MonoBehaviour
{
    // Effect özellikleri
    public ParticleSystem dust;

    // Hareket ve ziplama ozellikleri
    public float moveSpeed;

    // Yon kontrol degiskenleri
    public bool isFacingRight = true;

    // Animator bileseni
    private Animator animator;

    // Fiziksel ozellikleri yonetmek icin Rigidbody bileseni
    private Rigidbody2D rb;

    private ShiftController shiftController;
    private JumpController jumpController;
    private CrouchController crouchController;
    private Block block;
    private SlideController slideController;

    [HideInInspector]public float rayDirection;
    [HideInInspector]public Vector2 movement;

    // Baslangic metodu - Oyun basladiginda bir kere çalisir
    void Start()
    {
        shiftController = GetComponent<ShiftController>();
        jumpController = GetComponent<JumpController>();
        crouchController = GetComponent<CrouchController>();
        slideController = GetComponent<SlideController>();
        block = GetComponent<Block>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Guncelleme metodu - Her karede bir kere calisir
    void Update()
    {
        // Kullanici girislerini kontrol et
        MovementInput();
        CheckRayDirection();
    }

    void MovementInput()
    {
        if (slideController.isGroundSliding)
        {
            return;
        }
        if (crouchController.isCrouching || block.isBlocking)
        {
            moveSpeed = 2f;
        }else
        {
            moveSpeed = 5f;
        }
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        // If the character is dashing, update the velocity based on dash speed
        if (shiftController.isDashing)
        {
            movement = rb.velocity.normalized * shiftController.dashSpeed;
        }
        // Update the rigidbody's velocity with the movement vector
        rb.velocity = movement;

        // Karakterin yuzunu cevir
        FlipCharacter(horizontalInput);

        if (jumpController.isWallSliding)
        {
            movement = new Vector2(Input.GetAxis("Horizontal") * 0f, rb.velocity.y);
            rb.velocity = movement;
        }
        CreateDust(horizontalInput);
    }

    // Karakteri cevirme metodu
    void FlipCharacter(float horizontalInput)
    {
        // Karakterin yüzünü çevirme
        if ((horizontalInput < 0 && isFacingRight) || (horizontalInput > 0 && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void CheckRayDirection()
    {
        // Ray yönünü güncelle
        rayDirection = isFacingRight ? 1f : -1f;
    }
    void CreateDust(float horizontalInput)
    {
        if (Mathf.Abs(horizontalInput) > 0.1f && jumpController.isGrounded)
        {
            dust.Play();
        }
    }
}
