using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour
{
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

    [SerializeField] private int damageGiven = 50;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask attackMask;

    public float initialMoveSpeed = 5f; // Baþlangýçtaki hýz
    public float maxMoveSpeed = 15f; // Maksimum hýz

    private float currentMoveSpeed; // Anlýk hýz

    private PlayerMovement playerMovement;
    private JumpController jumpController;
    private Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        enabled = false;
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = initialMoveSpeed;
        jumpController = GetComponent<JumpController>();
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
    private void GiveDamage()
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