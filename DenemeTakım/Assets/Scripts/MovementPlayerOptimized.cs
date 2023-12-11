using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

// Oyuncu karakterinin hareketini kontrol eden C# script'i
public class MovementPlayer : MonoBehaviour
{
    // Hareket ve zýplama özellikleri
    public float moveSpeed;
    public float jumpForceMin;
    public float jumpForceMax;
    public float maxJumpTime;

    // Dash özellikleri
    public float dashDistance;
    public float dashDuration;

    // Dodge özellikleri
    public float dodgeDistance;
    public float dodgeDuration;

    // Yerde olup olmadýðýný kontrol etmek için Tilemap
    public Tilemap groundTilemap;

    // Animator bileþeni
    public Animator animator;

    // Fiziksel özellikleri yönetmek için Rigidbody bileþeni
    private Rigidbody2D rb;

    // Zýplama kontrol deðiþkenleri
    private bool isGrounded;
    private bool isJumping;
    private float jumpTime;
    private bool hasJumped;
    private int doubleJumpCount;

    // Dash kontrol deðiþkenleri
    private bool isDashing;
    private bool canDash = true;
    private bool isShifting;

    // Dodge kontrol deðiþkenleri
    private bool isDodging;
    private bool canDodge = true;

    // Yön kontrol deðiþkenleri
    private bool isFacingRight = true;

    // Bullet Time kontrol deðiþkenleri
    private bool isBulletTime;
    private float originalTimeScale;

    // Çift zýplama özellikleri
    public int maxDoubleJumps = 1;
    public float doubleJumpForce = 5f;

    // Dash hýzý
    public float dashSpeed = 10f;

   

    // Baþlangýç metodu - Oyun baþladýðýnda bir kere çalýþýr
    void Start()
    {
        // Rigidbody bileþenini al ve zaman ölçeðini kaydet
        rb = GetComponent<Rigidbody2D>();
        originalTimeScale = Time.timeScale;
    }

    // Güncelleme metodu - Her karede bir kere çalýþýr
    void Update()
    {
        // Kullanýcý giriþlerini kontrol et
        MovementInput();
        CheckGrounded();
        CheckJumpInput();
        CheckDashInput();
        CheckDodgeInput();
    }

    // Hareket giriþi iþleme metodu
    void MovementInput()
    {
        // Yatay giriþi al
        float horizontalInput = Input.GetAxis("Horizontal");
        // Hareket vektörünü oluþtur
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        // Animator'a hareket hýzýný iletiyoruz
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // Eðer karakter dash yapýyorsa, hareket vektörünü dash hýzýna göre ayarla
        if (isDashing)
        {
            movement = rb.velocity.normalized * dashSpeed;
        }

        // Hareketi uygula
        rb.velocity = movement;

        // Karakterin yüzünü çevir
        FlipCharacter(horizontalInput);
    }

    // Karakteri çevirme metodu
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

    // Karakterin yerde olup olmadýðýný kontrol etme metodu
    void CheckGrounded()
    {
        // Karakterin yerde olup olmadýðýný kontrol et
        Vector3Int cellPosition = groundTilemap.WorldToCell(transform.position - new Vector3(0f, 0.5f, 0f));
        isGrounded = groundTilemap.HasTile(cellPosition);

        // Karakter yerde deðilse ve düþüyorsa animasyonu baþlat
        if (!isGrounded)
        {
            animator.SetBool("isFalling", true);
        }

        // Karakter yerdeyse, zýplama durumunu sýfýrla ve düþüyorsa animasyonu durdur
        if (isGrounded)
        {
            hasJumped = false;
            doubleJumpCount = 0;
            animator.SetBool("isFalling", false);
        }
    }

    // Zýplama giriþini kontrol etme metodu
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
        if (Input.GetButton("Jump") && isJumping && jumpTime < maxJumpTime)
        {
            jumpTime += Time.deltaTime;
            float jumpForce = Mathf.Lerp(jumpForceMin, jumpForceMax, jumpTime / maxJumpTime);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // "Jump" tuþu býrakýldýðýnda
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            jumpTime = 0f;
            animator.SetBool("isJumping", false);
        }
    }

    // Dash giriþini kontrol etme metodu
    void CheckDashInput()
    {
        // "Dash" tuþuna basýldýðýnda ve dash kullanýlabilir durumdaysa
        if (Input.GetButtonDown("Dash") && canDash)
        {
            // Eðer shift yapýlmamýþsa
            if (!isShifting)
            {
                // Shift yap ve Bullet Time'ý aç
                isShifting = true;
                ToggleBulletTime();
            }
            // Eðer shift yapýlmýþsa
            else
            {
                // Shift'i kapat ve dash yönelimine göre Dash Coroutine'ý baþlat
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

    // Bullet Time'ý açma/kapatma metodu
    void ToggleBulletTime()
    {
        // Eðer Bullet Time açýk deðilse
        if (!isBulletTime)
        {
            // Bullet Time'ý aç ve zaman ölçeðini düþür
            isBulletTime = true;
            Time.timeScale = 0.2f;
        }
        // Eðer Bullet Time açýksa
        else
        {
            // Bullet Time'ý kapat ve zaman ölçeðini orijinal deðere geri getir
            isBulletTime = false;
            Time.timeScale = originalTimeScale;
        }
    }

    // Dodge giriþini kontrol etme metodu
    void CheckDodgeInput()
    {
        // "Dodge" tuþuna basýldýðýnda ve dodge kullanýlabilir durumdaysa
        if (Input.GetButtonDown("Dodge") && canDodge)
        {
            // Dodge Coroutine'ýný baþlat
            StartCoroutine(Dodge());
        }
    }

    // Dash iþlemini gerçekleþtiren Coroutine metodu
    IEnumerator Dash(Vector2 dashDirection)
    {
        // Dash animasyonunu baþlat ve dash durumunu aktifleþtir
        animator.SetBool("isDashing", true);
        isDashing = true;
        canDash = false;

        // Player ve enemyLayer'ýn çarpýþmalarýný geçici olarak ihmal et
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("enemyLayer");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // Dash hýzýnda hareket et
        rb.velocity = dashDirection * dashDistance / dashDuration;

        // Dash süresi kadar bekle
        yield return new WaitForSeconds(dashDuration);

        // Collision'larý tekrar aktifleþtir
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        // Dash durumunu kapat, animasyonu kapat ve hýzý sýfýrla
        isDashing = false;
        animator.SetBool("isDashing", false);
        rb.velocity = Vector2.zero;

        // Bir sonraki Dash'in yapýlabilmesi için bekle
        yield return new WaitForSeconds(1f);
        canDash = true;
    }

    // Dodge iþlemini gerçekleþtiren Coroutine metodu
    IEnumerator Dodge()
    {
        // Dodge animasyonunu baþlat ve dodge durumunu aktifleþtir
        isDodging = true;
        canDodge = false;

        // Dodge yönünü belirle
        float dodgeDirection = isFacingRight ? -1f : 1f;
        Vector2 dodgeVelocity = new Vector2(dodgeDirection, 0f).normalized;

        // Dodge hareketini hesapla
        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + dodgeVelocity * dodgeDistance;

        // Dodge süresince lerp (lineer interpolation) kullanarak hareket et
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
