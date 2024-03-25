using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{

    [Header("Effect Variables")]// Effect özellikler
    public ParticleSystem dust;

    [Header("Jump Variables")]//Ziplama ozellikleri
    public float jumpForceMin = 1f;
    public float jumpForceMax = 5f;
    [SerializeField] float maxJumpTime = 0.1f;

    [HideInInspector] public bool isGrounded;
    private bool isJumping;
    private float jumpTime;
    private bool hasJumped; 
    private int doubleJumpCount;

    private float rbVelocity;
    private bool canJump = true;

    [Header("Double Jump Variables")]// Çift ziplama ozellikleri
    [SerializeField] int maxDoubleJumps = 1;
    [SerializeField] float doubleJumpForce = 5f;

    // Animator bileseni
    private Animator animator;

    //PlayerMovement bileseni
    private PlayerMovement playerMovement;

    private ShiftController shiftController;

    //CapsuleCollider bileseni
    private CapsuleCollider2D capsuleCollider2d;

    [Header("LayerMask")]//LayerMask Bileseni
    [SerializeField] LayerMask groundlayerMask;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask ledgeLayerMask;


    //Duvar kontrol degiskeni
    private bool isTouchingWall;
    [HideInInspector] public bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private PlayerHealth playerHealth;
    private int fallDamage = 5;
    private int fallDamageRadius = 5;

    private Rigidbody2D rb;

    // Baslangic metodu - Oyun basladiginda bir kere çalisir
    void Start()
    {
        animator = GetComponent<Animator>(); //Animator Caching
        capsuleCollider2d = GetComponent<CapsuleCollider2D>(); //CapsuleCollider Caching
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        shiftController = GetComponent<ShiftController>();
    }

    void Update()
    {
        CheckJumpInput();
        CheckFalling();
        PerformWallSliding();
        CheckGrounded();

    }

    void FixedUpdate()
    { 
        CheckLedges();
        CheckWallSliding();
    }

    void CheckWallSliding()
    {
        RaycastHit2D raycastHitCenter = Physics2D.Raycast(new Vector2(capsuleCollider2d.bounds.center.x, capsuleCollider2d.bounds.center.y), Vector2.right * playerMovement.rayDirection, 0.4f, groundlayerMask);
        RaycastHit2D raycastHitTop = Physics2D.Raycast(new Vector2(capsuleCollider2d.bounds.center.x, capsuleCollider2d.bounds.max.y), Vector2.right * playerMovement.rayDirection, 0.4f, groundlayerMask);

        isTouchingWall = raycastHitTop.collider != null && raycastHitCenter.collider != null;

        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        } else
        {
            isWallSliding = false;
        }
    }
    void PerformWallSliding()
    {
        if (isWallSliding)
        {
            if (rb.velocity.y < wallSlidingSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
            }
        }
    }
    void CheckGrounded()
    {
        // Karakterin yerde olup olmadýðýný kontrol et
        RaycastHit2D raycastHit = Physics2D.Raycast(capsuleCollider2d.bounds.center, Vector2.down, capsuleCollider2d.bounds.extents.y + 0.2f, groundlayerMask);

        Color rayColor;

        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(capsuleCollider2d.bounds.center, Vector2.down * (capsuleCollider2d.bounds.extents.y + 0.2f), rayColor);
        isGrounded = raycastHit.collider != null;


        // Karakter yerdeyse, zýplama durumunu sýfýrla
        if (isGrounded)
        {
            hasJumped = false;
            doubleJumpCount = 0;
        }

    }

    void CheckJumpInput()
    {
        // "Jump" tuþuna basýldýðýnda
        if (Input.GetButtonDown("Jump") && canJump)
        {
            // Eðer yerdeyse
            if (isGrounded)
            {
                // Zýplama baþlat
                isJumping = true;
                jumpTime = 0f;
                rb.velocity = new Vector2(rb.velocity.x, jumpForceMin);
                animator.SetBool("isJumping", true);
            }
            // Yerde deðilse ve çift zýplama kullanýlabilirse
            else if (!hasJumped && doubleJumpCount < maxDoubleJumps)
            {
                // Çift zýplama baþlat
                hasJumped = true;
                doubleJumpCount++;
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                animator.SetBool("isJumping", true);
            }
        }

        // "Jump" tuþuna basýlý tutulduðu sürece ve zýplama zaman sýnýrýna ulaþýlmamýþsa
        if (Input.GetButton("Jump") && isJumping && jumpTime < maxJumpTime && canJump)
        {
            jumpTime += Time.deltaTime;
            float jumpForce = Mathf.Lerp(jumpForceMin, jumpForceMax, jumpTime / maxJumpTime); // Lineer Interpolasyon
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else
        {
            if (isGrounded)
            {
                animator.SetBool("isJumping", false);
            }
        }

        // "Jump" tuþu býrakýldýðýnda
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            jumpTime = 0f;
            animator.SetBool("isJumping", false);
        }
    }
    void CheckLedges()
    {
        RaycastHit2D raycastHitTop = Physics2D.Raycast(new Vector2(capsuleCollider2d.bounds.center.x, capsuleCollider2d.bounds.max.y), Vector2.right * playerMovement.rayDirection, 1f, ledgeLayerMask);

        Color rayColorTop;

        if (raycastHitTop.collider != null)
        {
            rayColorTop = Color.green;
        }
        else
        {
            rayColorTop = Color.red;
        }

        Debug.DrawRay(new Vector2(capsuleCollider2d.bounds.center.x, capsuleCollider2d.bounds.max.y), Vector2.right * playerMovement.rayDirection * 1f, rayColorTop);

        RaycastHit2D raycastHitCenter = Physics2D.Raycast(new Vector2(capsuleCollider2d.bounds.center.x, capsuleCollider2d.bounds.center.y), Vector2.right * playerMovement.rayDirection, 1f, ledgeLayerMask);

        Color rayColorCenter;

        if (raycastHitCenter.collider != null)
        {
            rayColorCenter = Color.green;
        }
        else
        {
            rayColorCenter = Color.red;
        }

        Debug.DrawRay(new Vector2(capsuleCollider2d.bounds.center.x, capsuleCollider2d.bounds.center.y), Vector2.right * playerMovement.rayDirection * 1f, rayColorCenter);

        //Tepedeki ray boþta ve merkezdeki ray collidera deðiyorsa ledge climb yap
        if (raycastHitTop.collider == null && raycastHitCenter.collider != null && !isGrounded)
        {
            PerformLedgeClimb();
        }
    }
    void PerformLedgeClimb()
    {
        Vector2 ledgeClimbPosition = new Vector2(capsuleCollider2d.bounds.center.x + playerMovement.rayDirection, capsuleCollider2d.bounds.max.y);
        transform.position = ledgeClimbPosition;
    }
    IEnumerator FallDamage() //30.01.2024
    {
        animator.SetBool("isFallDamaged", true);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isFallDamaged", false);
    }
    IEnumerator FallDamageMove() //30.01.2024
    {
        playerMovement.moveSpeed = 2f;
        yield return new WaitForSeconds(2f);
        playerMovement.moveSpeed = 5f;
    }
    void CheckFalling() //30.01.2024 Fall Damage eklendi.
    {
        rbVelocity = rb.velocity.y;

        if (rb.velocity.y < 0f)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }

        if (rb.velocity.y <= -15f && rb.velocity.y >= -20f && isGrounded && !shiftController.isDashing)
        {
            FallDamageController();
        }

        if (rb.velocity.y <= -20f && isGrounded && !shiftController.isDashing)
        {
            Debug.Log("Big Fall Damage");
        }
    }

    void FallDamageController()
    {
            // Patlama yarýçapý ve etkileþime girecek objeleri belirleme
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, fallDamageRadius, enemyLayer);

            foreach (Collider2D collider in colliders)
            {
                // Eðer düþman layer'ýna sahip bir objeyle temas edildiyse
                Health enemyHealth = collider.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    // Hasar verme iþlemini gerçekleþtir
                    enemyHealth.TakeDamage(20);
                }
            }
            playerHealth.TakeDamage(fallDamage);
            StartCoroutine(FallDamage());
            StartCoroutine(FallDamageMove());
            CreateDust();
    }

    void CreateDust() //03.02.2024 dust eklendi
    {
        // Dust particle'i calistir
        dust.Play();
    }

}