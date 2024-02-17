using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] AudioClip[] attackClips;
    [SerializeField] AudioClip hitClips;
    private int currentAttackClipsIndex = 0;
    private bool isHitted = false;
    private Animator animator;
    private bool isAttacking = false;
    private int comboCounter = 0;
    private int maxCombo = 3;
    private float attackCooldown = 3f;
    private float timeSinceLastAttack = 0f;

    [Header("Damage")]
    [SerializeField] private int damageGiven = 50;
    
    [Header("Attack Point")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask attackMask;

    private bool enemyUnder;
    
    [Header("Move During Attack Variables")]
    [SerializeField] private float initialMoveSpeed = 5f; // Baþlangýçtaki hýz
    [SerializeField] private float maxMoveSpeed = 15f; // Maksimum hýz

    private float currentMoveSpeed; // Anlýk hýz

    private PlayerMovement playerMovement;
    private JumpController jumpController;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider2d;

    private void Start()
    {
        animator = GetComponent<Animator>();
        enabled = false;
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = initialMoveSpeed;
        jumpController = GetComponent<JumpController>();
        capsuleCollider2d = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            if (comboCounter >= maxCombo || timeSinceLastAttack >= attackCooldown)
            {
                comboCounter = 0;
            }
            comboCounter++;
            StartCoroutine(PerformCombo());

        }
        currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, maxMoveSpeed, 0.01f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    void AttackSounds()
    {
        if (!isHitted)
        {
            AudioSource.PlayClipAtPoint(attackClips[currentAttackClipsIndex], transform.position);

            currentAttackClipsIndex = (currentAttackClipsIndex + 1) % attackClips.Length;
        }
    }
    void HitSound()
    {
        AudioSource.PlayClipAtPoint(hitClips,transform.position);
    }
    public void MoveForwardDuringAttack()
    {
        if (rb.velocity.x < 0.01f)
        {
            float horizontalSpeed = currentMoveSpeed * Mathf.Sign(transform.localScale.x);
            rb.velocity = new Vector2(horizontalSpeed, rb.velocity.y);
        }
    }
    private void CheckEnemy()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(capsuleCollider2d.bounds.center, Vector2.down, capsuleCollider2d.bounds.extents.y + 0.2f, attackMask);
        enemyUnder = raycastHit.collider != null;
    }
    private void GiveDamage() 
    /*metodunu kullanarak, "attackPoint" adlý pozisyondan belirli bir yarýçapa sahip bir dairesel alanda, 
    "attackMask" adlý katmanda yer alan tüm Collider'larý tespit eder. Bu, saldýrýnýn etki alanýný temsil eder.
    Bu bölgedeki tüm düþmanlarý temsil eden Collider'lar bir dizi içinde toplanýr (hitEnemies).*/
    {
        if (!enemyUnder)
        {
            // Hasar verme iþlemi
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackMask);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Health>().TakeDamage(damageGiven); // Hasar miktarýný ayarlayabilirsiniz
                isHitted = true;
                HitSound();
            }
        }
    }

    private IEnumerator PerformCombo()
    {
        isAttacking = true;
        timeSinceLastAttack = 0f;
        animator.SetTrigger(comboCounter + "Attack");
        AttackSounds();

        // Bekletme süresi sonrasýnda combo sýfýrla
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
        isHitted = false;
    }
}