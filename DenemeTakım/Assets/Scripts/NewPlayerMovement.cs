using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

// Oyuncu karakterinin hareketini kontrol eden C# script'i
public class NewPlayerMovement : MonoBehaviour
{
    // Hareket ve ziplama ozellikleri
    public float moveSpeed = 5f;
    public float jumpForceMin = 1f;
    public float jumpForceMax = 5f;
    public float maxJumpTime = 0.1f;

    // Dodge ozellikleri
    public float dodgeDistance = 0.5f;
    public float dodgeDuration = 0.1f;

    // Ziplama kontrol degiskenleri
    private bool isGrounded;
    private bool isJumping;
    private float jumpTime;
    private bool hasJumped;
    private int doubleJumpCount;

    // Dodge kontrol degiskenleri
    private bool isDodging;
    private bool canDodge = true;

    // Yon kontrol degiskenleri
    private bool isFacingRight = true;

    // Çift ziplama ozellikleri
    public int maxDoubleJumps = 1;
    public float doubleJumpForce = 5f;

    // Yerde olup olmadigini kontrol etmek için Tilemap
    public Tilemap groundTilemap;

    // Animator bileseni
    public Animator animator;

    // Fiziksel ozellikleri yonetmek icin Rigidbody bileseni
    public Rigidbody2D rb;

    public ShiftController shiftController;

    // Baslangic metodu - Oyun basladiginda bir kere çalisir
    void Start()
    {
    }
    void FixedUpdate()
    {
        // Rigidbody bilesenini al
        rb = GetComponent<Rigidbody2D>();
    }
    // Guncelleme metodu - Her karede bir kere calisir
    void Update()
    {
        // Kullanici girislerini kontrol et
        MovementInput();
        CheckGrounded();
        CheckJumpInput();
        CheckDodgeInput();
    }

    void MovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // If the character is dashing, update the velocity based on dash speed
        if (shiftController.isDashing)
        {
            movement = rb.velocity.normalized * shiftController.dashSpeed;
        }
        // Update the rigidbody's velocity with the movement vector
        rb.velocity = movement;

        // Karakterin yuzunu cevir
        FlipCharacter(horizontalInput);

    }

    // Karakteri cevirme metodu
    void FlipCharacter(float horizontalInput)
    {
        // Karakterin yuzunu çevirme
        if ((horizontalInput < 0 && isFacingRight) || (horizontalInput > 0 && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // Karakterin yerde olup olmadigini kontrol etme metodu
    void CheckGrounded()
    {
        // Karakterin yerde olup olmadigini kontrol et
        Vector3Int cellPosition = groundTilemap.WorldToCell(transform.position - new Vector3(0f, 0.5f, 0f));
        isGrounded = groundTilemap.HasTile(cellPosition);

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
    void CheckJumpInput()
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
                rb.velocity = new Vector2(rb.velocity.x, jumpForceMin);
                animator.SetBool("isJumping", true);
            }
            // Yerde degilse ve cift ziplama kullanilabilirse
            else if (!hasJumped && doubleJumpCount < maxDoubleJumps)
            {
                // Cift ziplama baslat
                hasJumped = true;
                doubleJumpCount++;
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                animator.SetBool("isJumping", true);
            }
        }

        // "Jump" tusuna basili tutuldugu surece ve ziplama zaman sinirina ulasilmamissa
        if (Input.GetButton("Jump") && isJumping && jumpTime < maxJumpTime)
        {
            jumpTime += Time.deltaTime;
            float jumpForce = Mathf.Lerp(jumpForceMin, jumpForceMax, jumpTime / maxJumpTime); //Lineer Interpolasyon
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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

    // Dodge girisini kontrol etme metodu
    void CheckDodgeInput()
    {
        // "Dodge" tusuna basildiginda ve dodge kullanilabilir durumdaysa
        if (Input.GetButtonDown("Dodge") && canDodge)
        {
            // Dodge Coroutine'ini baslat
            StartCoroutine(Dodge());
        }
    }
    // Dodge islemini gerceklestiren Coroutine metodu
    IEnumerator Dodge()
    {
        // Dodge animasyonunu baslat ve dodge durumunu aktiflestir
        isDodging = true;
        canDodge = false;

        // Dodge yonunu belirle
        float dodgeDirection = isFacingRight ? -1f : 1f;
        Vector2 dodgeVelocity = new Vector2(dodgeDirection, 0f).normalized;

        // Dodge hareketini hesapla
        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + dodgeVelocity * dodgeDistance;

        // Dodge suresince linner interpolasyon kullanarak hareket et
        float startTime = Time.time;
        while (Time.time < startTime + dodgeDuration)
        {
            float t = (Time.time - startTime) / dodgeDuration;
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
            yield return null;
        }

        // Dodge durumunu kapat ve bir sonraki Dodge için bekle
        isDodging = false;
        yield return new WaitForSeconds(1f);
        canDodge = true;
    }
}
