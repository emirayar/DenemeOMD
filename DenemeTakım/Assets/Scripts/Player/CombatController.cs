using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    private int currentAudioClipsIndex = 0;

    private Animator animator;
    private bool isAttacking = false;
    private int comboCounter = 0;
    private int maxCombo = 2;
    private float attackCooldown = 3f;
    private float timeSinceLastAttack = 0f;

    [SerializeField] private int damageGiven = 50;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask attackMask;

    public float initialMoveSpeed = 5f; // Ba�lang��taki h�z
    public float maxMoveSpeed = 15f; // Maksimum h�z

    private float currentMoveSpeed; // Anl�k h�z

    private PlayerMovement playerMovement;
    private Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        enabled = false;
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = initialMoveSpeed;
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
        AudioSource.PlayClipAtPoint(audioClips[currentAudioClipsIndex], transform.position);

        currentAudioClipsIndex = (currentAudioClipsIndex + 1) % audioClips.Length;
    }
    public void MoveForwardDuringAttack()
    {
        float horizontalSpeed = currentMoveSpeed * Mathf.Sign(transform.localScale.x);
        rb.velocity = new Vector2(horizontalSpeed, rb.velocity.y);
    }
    private void GiveDamage()
    {
        // Hasar verme i�lemi
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackMask);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Health>().TakeDamage(damageGiven); // Hasar miktar�n� ayarlayabilirsiniz
        }
    }

    private IEnumerator PerformCombo()
    {
        isAttacking = true;
        timeSinceLastAttack = 0f;
        animator.SetTrigger(comboCounter + "Attack");
        AttackSounds();

        // Bekletme s�resi sonras�nda combo s�f�rla
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;

    }
}