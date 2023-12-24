using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    //Ziplama ozellikleri
    public float jumpForceMin = 1f;
    public float jumpForceMax = 5f;
    public float maxJumpTime = 0.1f;

    // Ziplama kontrol degiskenleri
    private bool isGrounded;
    private bool isJumping;
    private float jumpTime;
    private bool hasJumped;
    private int doubleJumpCount;

    // Çift ziplama ozellikleri
    public int maxDoubleJumps = 1;
    public float doubleJumpForce = 5f;

    // Animator bileseni
    private Animator animator;

    //PlayerMovement bileseni
    public PlayerMovement playerMovement;
    // Baslangic metodu - Oyun basladiginda bir kere çalisir
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGrounded();
        CheckJumpInput();
        CheckFalling();

    }

    void CheckGrounded()
    {
        // Karakterin yerde olup olmadýðýný kontrol et
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f);

        isGrounded = playerMovement.rb.velocity.y <= 0 && hit.collider != null;

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
        if (Input.GetButtonDown("Jump"))
        {
            // Eðer yerdeyse
            if (isGrounded)
            {
                // Zýplama baþlat
                isJumping = true;
                jumpTime = 0f;
                playerMovement.rb.velocity = new Vector2(playerMovement.rb.velocity.x, jumpForceMin);
                animator.SetBool("isJumping", true);
            }
            // Yerde deðilse ve çift zýplama kullanýlabilirse
            else if (!hasJumped && doubleJumpCount < maxDoubleJumps)
            {
                // Çift zýplama baþlat
                hasJumped = true;
                doubleJumpCount++;
                playerMovement.rb.velocity = new Vector2(playerMovement.rb.velocity.x, doubleJumpForce);
                animator.SetBool("isJumping", true);
            }
        }

        // "Jump" tuþuna basýlý tutulduðu sürece ve zýplama zaman sýnýrýna ulaþýlmamýþsa
        if (Input.GetButton("Jump") && isJumping && jumpTime < maxJumpTime)
        {
            jumpTime += Time.deltaTime;
            float jumpForce = Mathf.Lerp(jumpForceMin, jumpForceMax, jumpTime / maxJumpTime); // Lineer Interpolasyon
            playerMovement.rb.velocity = new Vector2(playerMovement.rb.velocity.x, jumpForce);
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
    void CheckFalling()
    {
        if (playerMovement.rb.velocity.y < 0f)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
    }
}
