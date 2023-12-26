using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

// Oyuncu karakterinin hareketini kontrol eden C# script'i
public class PlayerMovement : MonoBehaviour
{
    // Hareket ve ziplama ozellikleri
    public float moveSpeed = 5f;

    // Dodge ozellikleri
    public float dodgeDistance = 0.5f;
    public float dodgeDuration = 0.1f;

    // Dodge kontrol degiskenleri
    private bool isDodging;
    private bool canDodge = true;

    // Yon kontrol degiskenleri
    private bool isFacingRight = true;

    // Animator bileseni
    public Animator animator;

    // Fiziksel ozellikleri yonetmek icin Rigidbody bileseni
    public Rigidbody2D rb;

    public ShiftController shiftController;
    public JumpController jumpController;

    public float rayDirection;
    public Vector2 movement;

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
        CheckDodgeInput();
        CheckRayDirection();
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
            
            if (jumpController.isWallSliding)
            {
                movement = new Vector2(Input.GetAxis("Horizontal") * 0f, rb.velocity.y);
                rb.velocity = movement;
        }
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
    void CheckRayDirection()
    {
        // Ray yönünü güncelle
        rayDirection = isFacingRight ? 1f : -1f;
    }

}
