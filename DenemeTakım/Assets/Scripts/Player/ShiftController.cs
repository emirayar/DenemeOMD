using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftController : MonoBehaviour
{
    [Header("Dash Variables")]// Dash ozellikleri
    [SerializeField] float dashDistance = 20f;
    [SerializeField] float dashDuration = 0.2f;
    public float dashSpeed = 25f;

    [Header("Audio Clip")]
    [SerializeField] AudioClip dashClip;

    // Dash kontrol degiskenleri
    [HideInInspector] public bool isDashing;
    private bool canDash = true;

    // Bullet Time kontrol degiskenleri
    private bool isBulletTime;
    private float originalTimeScale;

    //TrailRenderer bilesenini aliriz
    private TrailRenderer trailRenderer;

    // Arrow Controller scriptini ekleriz.
    private ControllerArrow arrow;

    // Animator bileseni
    private Animator animator;

    private JumpController jumpController;
    
    private Vector2 dashDirection;

    private Rigidbody2D rb;

    private LogManager logManager;

    private float bulletTimeTimer;

    private Stamina stamina;

    // Baslangic metodu - Oyun basladiginda bir kere �alisir
    void Start()
    {
        originalTimeScale = Time.timeScale;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false; //Baslangicta Trail'imiz kapali olur.
        animator = GetComponent<Animator>();
        arrow = GetComponentInChildren<ControllerArrow>(); 
        jumpController = GetComponent<JumpController> ();
        rb = GetComponent<Rigidbody2D>();
        logManager = GetComponentInChildren<LogManager>();
        stamina = GetComponent<Stamina>();
    }
    void Update()
    {
        // Bullet Time s�resi dolmu�sa, Bullet Time'� kapat
        if (isBulletTime && Time.time - bulletTimeTimer >= 1f)
        {
            ToggleBulletTime();
            DashAction();
        }

        CheckDashInput();
    }


    private void CheckDashInput()
    {
        // "Dash" tusuna basildiginda ve dash kullanilabilir durumdaysa
        if (Input.GetButtonDown("Dash") && canDash && !jumpController.isWallSliding)
        {
            // E�er stamina 20'nin alt�ndaysa dash i�lemi ger�ekle�tirme
            if (stamina.currentStamina >= 20)
            {
                //Bullet Time'i a�
                ToggleBulletTime();
                // Bullet Time ba�lang�� zaman�n� kaydet
                bulletTimeTimer = Time.time;
            }
            else
            {
                Debug.Log("Not enough stamina");
                stamina.DecreasingEffect();
            }
        }

        // "Dash" tusu birakildiginda
        if (Input.GetButtonUp("Dash"))
        {
            // Eger shift yapilmis ve Bullet Time aciksa
            if (isBulletTime)
            {
                //Bullet Time'i kapat ve dash yonelimine gore Dash Coroutine'i baslat
                ToggleBulletTime();

                DashAction();

            }
        }

    }

    void DashAction()
    {
        // Karakter yerdeyken, alt, sa� alt ve sol alt y�nlerde dash yapmas�n� engelle
        if (jumpController.isGrounded)
        {
            Vector2 dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

            if (dashDirection.y >= 0) // Y ekseninde yukar�ya do�ru bir hareket varsa
            {
                StartCoroutine(Dash(dashDirection));
            }
            else if (dashDirection.y < 0)
            {
                logManager.Log("You can't dash to ground");
            }
        }
        else // Karakter yerde de�ilse herhangi bir s�n�rlama yapma
        {
            Vector2 dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

            if (dashDirection != Vector2.zero)
            {
                StartCoroutine(Dash(dashDirection));
            }
        }
    }

    // Bullet Time'i acma/kapatma metodu
    void ToggleBulletTime()
    {
        // Eger Bullet Time acik degilse
        if (!isBulletTime)
        {
            // Bullet Time'i ac, zaman olcegini dusur ve Trail'i baslat.
            isBulletTime = true;
            Time.timeScale = 0.2f;
            trailRenderer.emitting = true;
            arrow.enabled = true;
            arrow.GetComponent<SpriteRenderer>().enabled = true;
        }
        // Eger Bullet Time aciksa
        else
        {
            // Bullet Time'i kapat, zaman olcegini orijinal degere geri getir ve Trail'i kapat.
            isBulletTime = false;
            Time.timeScale = originalTimeScale;
            trailRenderer.emitting = false;
            arrow.enabled = false;
            arrow.GetComponent<SpriteRenderer>().enabled = false;
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
        rb.velocity = dashDirection * (dashDistance / dashDuration);

        stamina.UseStamina(20);

        // Dash suresi kadar bekle
        yield return new WaitForSeconds(dashDuration);

        // Collision'lari tekrar aktiflestir
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        // Dash durumunu kapat, animasyonu kapat ve hizi sifirla
        isDashing = false;
        animator.SetBool("isDashing", false);
        rb.velocity = Vector2.zero;
        AudioSource.PlayClipAtPoint(dashClip, transform.position);

        while (!jumpController.isGrounded)
        {
            yield return null;
        }
        canDash = true;
    }
}