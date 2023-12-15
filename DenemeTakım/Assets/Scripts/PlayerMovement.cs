using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

// Oyuncu karakterinin hareketini kontrol eden C# script'i
public class PlayerMovement : MonoBehaviour
{
    // Hareket ve ziplama ozellikleri
    public float moveSpeed;
    public float jumpForceMin;
    public float jumpForceMax;
    public float maxJumpTime;

    // Dash ozellikleri
    public float dashDistance;
    public float dashDuration;

    // Dodge ozellikleri
    public float dodgeDistance;
    public float dodgeDuration;

    // Yerde olup olmadigini kontrol etmek için Tilemap
    public Tilemap groundTilemap;

    // Animator bileseni
    public Animator animator;

    // Fiziksel ozellikleri yonetmek icin Rigidbody bileseni
    private Rigidbody2D rb;

    // Ziplama kontrol degiskenleri
    private bool isGrounded;
    private bool isJumping;
    private float jumpTime;
    private bool hasJumped;
    private int doubleJumpCount;

    // Dash kontrol degiskenleri
    private bool isDashing;
    private bool canDash = true;
    private bool isShifting;

    // Dodge kontrol degiskenleri
    private bool isDodging;
    private bool canDodge = true;

    // Yon kontrol degiskenleri
    private bool isFacingRight = true;

    // Bullet Time kontrol degiskenleri
    private bool isBulletTime;
    private float originalTimeScale;

    // Çift ziplama ozellikleri
    public int maxDoubleJumps = 1;
    public float doubleJumpForce = 5f;

    // Dash hizi
    public float dashSpeed = 10f;

   

    // Baslangic metodu - Oyun basladiginda bir kere çalisir
    void Start()
    {
        originalTimeScale = Time.timeScale;
    }
    void FixedUpdate()
    {
        // Rigidbody bilesenini al ve zaman olcegini kaydet
        rb = GetComponent<Rigidbody2D>();
    }
    // Guncelleme metodu - Her karede bir kere calisir
    void Update()
    {
        // Kullanici girislerini kontrol et
        MovementInput();
        CheckGrounded();
        CheckJumpInput();
        CheckDashInput();
        CheckDodgeInput();
        
    }

    // Hareket girisi isleme metodu
    void MovementInput()
    {
        // Yatay girisi al
        float horizontalInput = Input.GetAxis("Horizontal");
        // Hareket vektorunu olustur
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        // Animator'a hareket hizini iletiyoruz
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // Eger karakter dash yapiyorsa, hareket vektorunu dash hizina gore ayarla
        if (isDashing)
        {
            movement = rb.velocity.normalized * dashSpeed;
        }

        // Hareketi uygula
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

    // Dash girisini kontrol etme metodu
    void CheckDashInput()
    {
        // "Dash" tusuna basildiginda ve dash kullanilabilir durumdaysa
        if (Input.GetButtonDown("Dash") && canDash)
        {
            // Eger shift yapilmamissa
            if (!isShifting)
            {
                //Bullet Time'i aç
                isShifting = true;
                ToggleBulletTime();
            }
            // Eger shift yapilmissa
            else
            {
                //Bullet Time'i kapat ve dash yonelimine gore Dash Coroutine'i baslat
                isShifting = false;
                ToggleBulletTime();
                Vector2 dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

                if (dashDirection != Vector2.zero)
                {
                    StartCoroutine(Dash(dashDirection));
                }
            }
        }
    }

    // Bullet Time'i acma/kapatma metodu
    void ToggleBulletTime()
    {
        // Eger Bullet Time acik degilse
        if (!isBulletTime)
        {
            // Bullet Time'i ac ve zaman olcegini dusur
            isBulletTime = true;
            Time.timeScale = 0.2f;
        }
        // Eger Bullet Time aciksa
        else
        {
            // Bullet Time'i kapat ve zaman olcegini orijinal degere geri getir
            isBulletTime = false;
            Time.timeScale = originalTimeScale;
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

    // Dash islemini gerceklestiren Coroutine metodu
    IEnumerator Dash(Vector2 dashDirection)
    {
        // Dash animasyonunu baslat ve dash durumunu aktiflestir
        animator.SetBool("isDashing", true);
        isDashing = true;
        canDash = false;

        // Player ve enemyLayer'in carpismalarini gecici olarak ihmal et
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("enemyLayer");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // Dash hizinda hareket et
        rb.velocity = dashDirection * dashDistance / dashDuration;

        // Dash suresi kadar bekle
        yield return new WaitForSeconds(dashDuration);

        // Collision'lari tekrar aktiflestir
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        // Dash durumunu kapat, animasyonu kapat ve hizi sifirla
        isDashing = false;
        animator.SetBool("isDashing", false);
        rb.velocity = Vector2.zero;

        // Bir sonraki Dash'in yapilabilmesi için bekle
        yield return new WaitForSeconds(1f);
        canDash = true;
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
