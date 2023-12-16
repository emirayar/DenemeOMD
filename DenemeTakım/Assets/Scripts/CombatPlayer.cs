using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPlayer : MonoBehaviour
{
    public Animator animator;
    public float comboTime = 0.7f;
    public float moveSpeedDuringCombo = 1f;
    public float attackRange = 2f;

    private float comboTimer;
    private bool isComboActive = false;
    private bool isFacingRight = true;

    private Transform enemyTransform;

    void Start()
    {
        enabled = false;

        GameObject enemyObject = GameObject.Find("Enemy");
        if (enemyObject != null)
        {
            enemyTransform = enemyObject.transform;
        }
        else
        {
            Debug.LogError("Düşman nesnesi bulunamadı.");
        }
    }

    void Update()
    {
        UpdateFacingDirection();

        if (Input.GetMouseButtonDown(0))
        {
            if (isComboActive)
            {
                Attack2();
            }
            else
            {
                Attack();
            }
        }

        if (isComboActive)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }

            if (isFacingRight)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }
    }

    void Attack()
    {
        
        Vector3 enemyPosition = enemyTransform.position;

        // Calculate the direction towards the enemy.
        Vector3 direction = enemyPosition - transform.position;

        // Normalize the direction vector.
        direction.Normalize();

        // Oyuncunun yeni pozisyonunu hesaplayın.
        Vector3 newPosition = transform.position + direction * attackRange;

        // Move the player towards the enemy.
        transform.position = Vector3.MoveTowards(transform.position, newPosition, moveSpeedDuringCombo * Time.deltaTime);

        animator.SetTrigger("Attack");

        // Combo zamanlayıcısını başlatın ve combo'yu etkin olarak işaretleyin.
        comboTimer = comboTime;
        isComboActive = true;
        animator.SetBool("ComboTime", true);
    }

    void Attack2()
    {
        // Düşmanın pozisyonunu belirleyin (bu referansı ayarlamanız gerekir).
        Vector3 enemyPosition = enemyTransform.position;

        // Calculate the direction towards the enemy.
        Vector3 direction = enemyPosition - transform.position;

        // Normalize the direction vector.
        direction.Normalize();

        // Oyuncunun yeni pozisyonunu hesaplayın.
        Vector3 newPosition = transform.position + direction * attackRange;

        // Move the player towards the enemy.
        transform.position = Vector3.MoveTowards(transform.position, newPosition, moveSpeedDuringCombo * Time.deltaTime);

        animator.SetTrigger("Attack2");

        // Combo zamanlayıcısını başlatın ve combo'yu etkin olarak işaretleyin.
        comboTimer = comboTime;
        isComboActive = true;
        animator.SetBool("ComboTime", true);
    }

    void ResetCombo()
    {
        isComboActive = false;
        animator.SetBool("ComboTime", false);
    }

    void MoveRight()
    {
        transform.Translate(Vector3.right * moveSpeedDuringCombo * Time.deltaTime);
    }

    void MoveLeft()
    {
        transform.Translate(Vector3.left * moveSpeedDuringCombo * Time.deltaTime);
    }

    void UpdateFacingDirection()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            isFacingRight = true;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            isFacingRight = false;
        }

        if (isFacingRight)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
