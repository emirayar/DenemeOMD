using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideController : MonoBehaviour
{
    private Rigidbody2D rb;
    private ShiftController shiftController;
    private PlayerMovement playerMovement;
    private JumpController jumpController;
    private CrouchController crouchController;
    private CapsuleCollider2D capsuleCollider;
    public CapsuleCollider2D slideCollider;
    private Animator animator;
    private Stamina stamina;

    public bool isGroundSliding = false;

    private bool canSlide = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        shiftController = GetComponent<ShiftController> ();
        jumpController = GetComponent<JumpController> ();
        crouchController = GetComponent<CrouchController> ();
        capsuleCollider = GetComponent<CapsuleCollider2D> ();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        stamina = GetComponent<Stamina>();

        slideCollider.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (stamina.currentStamina >= 20)
        {
            Slide();
        }else
        {
            Debug.Log("Not Enough Stamina");
            stamina.DecreasingEffect();
        }

    }

    void Slide()
    {
        if (Input.GetButton("Slide") && canSlide)
        {
            if (Mathf.Abs(rb.velocity.x) > 2f && rb.velocity.y < 0.1f && jumpController.isGrounded && !crouchController.isCrouching)
            {
                isGroundSliding = true;
                // Kayma animasyonunu çalýþtýr
                animator.SetBool("isSliding", true);

                capsuleCollider.enabled = false;
                slideCollider.enabled = true;
                if (playerMovement.isFacingRight)
                {
                    rb.AddForce(Vector2.right * 0.5f, ForceMode2D.Impulse);
                }
                else
                {
                    rb.AddForce(Vector2.right * -1 * 0.5f, ForceMode2D.Impulse);
                }
                stamina.UseStamina(20);

                int playerLayer = LayerMask.NameToLayer("Player");
                int enemyLayer = LayerMask.NameToLayer("enemyLayer");
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

                StartCoroutine(StopSliding());
                StartCoroutine(SlideCooldown());
            }
        }
    }
    private IEnumerator StopSliding()
    {
        yield return new WaitForSeconds(0.4f);
        isGroundSliding = false;
        // Kayma animasyonunu durdur
        animator.SetBool("isSliding", false);

        capsuleCollider.enabled = true;
        slideCollider.enabled = false;

        // Ýki collider arasýndaki çarpýþmayý tekrar aktif et
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("enemyLayer");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }
    private IEnumerator SlideCooldown()
    {
        canSlide = false;

        yield return new WaitForSeconds(1f);

        canSlide = true;
    }
}