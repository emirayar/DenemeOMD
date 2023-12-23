/*using System.Collections;
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
    // Karakterin yerde olup olmadigini kontrol etme metodu
    public void CheckGrounded()
    {
        // Karakterin yerde olup olmadigini kontrol et
        Vector3Int cellPosition = playerMovement.groundTilemap.WorldToCell(transform.position - new Vector3(0f, 0.5f, 0f));
        isGrounded = playerMovement.groundTilemap.HasTile(cellPosition);

        // Karakter yerde degilse ve dusuyorsa animasyonu baþlat
        if (!isGrounded)
        {
            animator.SetBool("isFalling", true);
        }

        // Karakter yerdeyse, ziplama durumunu sifirla ve dusuyorsa animasyonu durdur
        if (isGrounded)
        {
            hasJumped = false;
            doubleJumpCount = 0;
            animator.SetBool("isFalling", false);
        }
    }

    // Ziplama girisini kontrol etme metodu
    public void CheckJumpInput()
    {
        // "Jump" tusuna basildiginda
        if (Input.GetButtonDown("Jump"))
        {
            // Eger yerdeyse
            if (isGrounded)
            {
                // Ziplama baslat
                isJumping = true;
                jumpTime = 0f;
                playerMovement.rb.velocity = new Vector2(playerMovement.rb.velocity.x, jumpForceMin);
                animator.SetBool("isJumping", true);
            }
            // Yerde degilse ve cift ziplama kullanilabilirse
            else if (!hasJumped && doubleJumpCount < maxDoubleJumps)
            {
                // Cift ziplama baslat
                hasJumped = true;
                doubleJumpCount++;
                playerMovement.rb.velocity = new Vector2(playerMovement.rb.velocity.x, doubleJumpForce);
                animator.SetBool("isJumping", true);
            }
        }

        // "Jump" tusuna basili tutuldugu surece ve ziplama zaman sinirina ulasilmamissa
        if (Input.GetButton("Jump") && isJumping && jumpTime < maxJumpTime)
        {
            jumpTime += Time.deltaTime;
            float jumpForce = Mathf.Lerp(jumpForceMin, jumpForceMax, jumpTime / maxJumpTime); //Lineer Interpolasyon
            playerMovement.rb.velocity = new Vector2(playerMovement.rb.velocity.x, jumpForce);
        }
        else
        {
            if (isGrounded)
            {
                animator.SetBool("isJumping", false);
            }
        }

        // "Jump" tusu birakildiginda
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            jumpTime = 0f;
            animator.SetBool("isJumping", false);
        }
    }

}
*/