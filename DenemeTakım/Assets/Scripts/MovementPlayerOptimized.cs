using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

// Oyuncu karakterinin hareketini kontrol eden C# script'i
public class MovementPlayer : MonoBehaviour
{
    // Hareket ve z�plama �zellikleri
    public float moveSpeed;
    public float jumpForceMin;
    public float jumpForceMax;
    public float maxJumpTime;

    // Dash �zellikleri
    public float dashDistance;
    public float dashDuration;

    // Dodge �zellikleri
    public float dodgeDistance;
    public float dodgeDuration;

    // Yerde olup olmad���n� kontrol etmek i�in Tilemap
    public Tilemap groundTilemap;

    // Animator bile�eni
    public Animator animator;

    // Fiziksel �zellikleri y�netmek i�in Rigidbody bile�eni
    private Rigidbody2D rb;

    // Z�plama kontrol de�i�kenleri
    private bool isGrounded;
    private bool isJumping;
    private float jumpTime;
    private bool hasJumped;
    private int doubleJumpCount;

    // Dash kontrol de�i�kenleri
    private bool isDashing;
    private bool canDash = true;
    private bool isShifting;

    // Dodge kontrol de�i�kenleri
    private bool isDodging;
    private bool canDodge = true;

    // Y�n kontrol de�i�kenleri
    private bool isFacingRight = true;

    // Bullet Time kontrol de�i�kenleri
    private bool isBulletTime;
    private float originalTimeScale;

    // �ift z�plama �zellikleri
    public int maxDoubleJumps = 1;
    public float doubleJumpForce = 5f;

    // Dash h�z�
    public float dashSpeed = 10f;

   

    // Ba�lang�� metodu - Oyun ba�lad���nda bir kere �al���r
    void Start()
    {
        // Rigidbody bile�enini al ve zaman �l�e�ini kaydet
        rb = GetComponent<Rigidbody2D>();
        originalTimeScale = Time.timeScale;
    }

    // G�ncelleme metodu - Her karede bir kere �al���r
    void Update()
    {
        // Kullan�c� giri�lerini kontrol et
        MovementInput();
        CheckGrounded();
        CheckJumpInput();
        CheckDashInput();
        CheckDodgeInput();
    }

    // Hareket giri�i i�leme metodu
    void MovementInput()
    {
        // Yatay giri�i al
        float horizontalInput = Input.GetAxis("Horizontal");
        // Hareket vekt�r�n� olu�tur
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        // Animator'a hareket h�z�n� iletiyoruz
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // E�er karakter dash yap�yorsa, hareket vekt�r�n� dash h�z�na g�re ayarla
        if (isDashing)
        {
            movement = rb.velocity.normalized * dashSpeed;
        }

        // Hareketi uygula
        rb.velocity = movement;

        // Karakterin y�z�n� �evir
        FlipCharacter(horizontalInput);
    }

    // Karakteri �evirme metodu
    void FlipCharacter(float horizontalInput)
    {
        // Karakterin y�z�n� �evirme
        if ((horizontalInput < 0 && isFacingRight) || (horizontalInput > 0 && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // Karakterin yerde olup olmad���n� kontrol etme metodu
    void CheckGrounded()
    {
        // Karakterin yerde olup olmad���n� kontrol et
        Vector3Int cellPosition = groundTilemap.WorldToCell(transform.position - new Vector3(0f, 0.5f, 0f));
        isGrounded = groundTilemap.HasTile(cellPosition);

        // Karakter yerde de�ilse ve d���yorsa animasyonu ba�lat
        if (!isGrounded)
        {
            animator.SetBool("isFalling", true);
        }

        // Karakter yerdeyse, z�plama durumunu s�f�rla ve d���yorsa animasyonu durdur
        if (isGrounded)
        {
            hasJumped = false;
            doubleJumpCount = 0;
            animator.SetBool("isFalling", false);
        }
    }

    // Z�plama giri�ini kontrol etme metodu
    void CheckJumpInput()
    {
        // "Jump" tu�una bas�ld���nda
        if (Input.GetButtonDown("Jump"))
        {
            // E�er yerdeyse
            if (isGrounded)
            {
                // Z�plama ba�lat
                isJumping = true;
                jumpTime = 0f;
                rb.velocity = new Vector2(rb.velocity.x, jumpForceMin);
                animator.SetBool("isJumping", true);
            }
            // Yerde de�ilse ve �ift z�plama kullan�labilirse
            else if (!hasJumped && doubleJumpCount < maxDoubleJumps)
            {
                // �ift z�plama ba�lat
                hasJumped = true;
                doubleJumpCount++;
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                animator.SetBool("isJumping", true);
            }
        }

        // "Jump" tu�una bas�l� tutuldu�u s�rece ve z�plama zaman s�n�r�na ula��lmam��sa
        if (Input.GetButton("Jump") && isJumping && jumpTime < maxJumpTime)
        {
            jumpTime += Time.deltaTime;
            float jumpForce = Mathf.Lerp(jumpForceMin, jumpForceMax, jumpTime / maxJumpTime);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // "Jump" tu�u b�rak�ld���nda
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            jumpTime = 0f;
            animator.SetBool("isJumping", false);
        }
    }

    // Dash giri�ini kontrol etme metodu
    void CheckDashInput()
    {
        // "Dash" tu�una bas�ld���nda ve dash kullan�labilir durumdaysa
        if (Input.GetButtonDown("Dash") && canDash)
        {
            // E�er shift yap�lmam��sa
            if (!isShifting)
            {
                // Shift yap ve Bullet Time'� a�
                isShifting = true;
                ToggleBulletTime();
            }
            // E�er shift yap�lm��sa
            else
            {
                // Shift'i kapat ve dash y�nelimine g�re Dash Coroutine'� ba�lat
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

    // Bullet Time'� a�ma/kapatma metodu
    void ToggleBulletTime()
    {
        // E�er Bullet Time a��k de�ilse
        if (!isBulletTime)
        {
            // Bullet Time'� a� ve zaman �l�e�ini d���r
            isBulletTime = true;
            Time.timeScale = 0.2f;
        }
        // E�er Bullet Time a��ksa
        else
        {
            // Bullet Time'� kapat ve zaman �l�e�ini orijinal de�ere geri getir
            isBulletTime = false;
            Time.timeScale = originalTimeScale;
        }
    }

    // Dodge giri�ini kontrol etme metodu
    void CheckDodgeInput()
    {
        // "Dodge" tu�una bas�ld���nda ve dodge kullan�labilir durumdaysa
        if (Input.GetButtonDown("Dodge") && canDodge)
        {
            // Dodge Coroutine'�n� ba�lat
            StartCoroutine(Dodge());
        }
    }

    // Dash i�lemini ger�ekle�tiren Coroutine metodu
    IEnumerator Dash(Vector2 dashDirection)
    {
        // Dash animasyonunu ba�lat ve dash durumunu aktifle�tir
        animator.SetBool("isDashing", true);
        isDashing = true;
        canDash = false;

        // Player ve enemyLayer'�n �arp��malar�n� ge�ici olarak ihmal et
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("enemyLayer");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // Dash h�z�nda hareket et
        rb.velocity = dashDirection * dashDistance / dashDuration;

        // Dash s�resi kadar bekle
        yield return new WaitForSeconds(dashDuration);

        // Collision'lar� tekrar aktifle�tir
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        // Dash durumunu kapat, animasyonu kapat ve h�z� s�f�rla
        isDashing = false;
        animator.SetBool("isDashing", false);
        rb.velocity = Vector2.zero;

        // Bir sonraki Dash'in yap�labilmesi i�in bekle
        yield return new WaitForSeconds(1f);
        canDash = true;
    }

    // Dodge i�lemini ger�ekle�tiren Coroutine metodu
    IEnumerator Dodge()
    {
        // Dodge animasyonunu ba�lat ve dodge durumunu aktifle�tir
        isDodging = true;
        canDodge = false;

        // Dodge y�n�n� belirle
        float dodgeDirection = isFacingRight ? -1f : 1f;
        Vector2 dodgeVelocity = new Vector2(dodgeDirection, 0f).normalized;

        // Dodge hareketini hesapla
        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + dodgeVelocity * dodgeDistance;

        // Dodge s�resince lerp (lineer interpolation) kullanarak hareket et
        float startTime = Time.time;
        while (Time.time < startTime + dodgeDuration)
        {
            float t = (Time.time - startTime) / dodgeDuration;
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
            yield return null;
        }

        // Dodge durumunu kapat ve bir sonraki Dodge i�in bekle
        isDodging = false;
        yield return new WaitForSeconds(1f);
        canDodge = true;
    }
}
