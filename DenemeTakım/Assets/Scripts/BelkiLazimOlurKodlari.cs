/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BelkiLazimOlurKodlari : MonoBehaviour
{
    public AudioClip[] audioClips;
    private int currentAudioClipsIndex = 0;
    
    void AttackSounds()
    {
        if (audioClips.Length > 0)
        {
            AudioSource.PlayClipAtPoint(audioClips[currentAudioClipsIndex], transform.position);

            currentAudioClipsIndex = (currentAudioClipsIndex + 1) % audioClips.Length;
        }
    }
}
*/

//isGrounded = playerMovement.rb.velocity.y <= 0;

/*
    // Dodge ozellikleri
    public float dodgeDistance = 0.5f;
    public float dodgeDuration = 0.1f;

    // Dodge kontrol degiskenleri
    private bool isDodging;
    private bool canDodge = true;


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
    } */

/*
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float detectionRadius = 5.0f;
    public float attackRange = 2.0f;
    public float attackCooldown = 1.0f;
    public float patrolSpeed = 1.0f;
    public Animator animator;

    private GameObject target;
    private GameObject[] patrolPoints;
    private int currentPatrolPointIndex = 0;
    private bool isPatrolling = true;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    private float attackCooldownTimer = 0.0f;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");

        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
    }

    private void Update()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

            if (isPatrolling)
            {
                if (distanceToTarget <= detectionRadius)
                {
                    isPatrolling = false;
                }
                else
                {
                    Patrol();
                }
            }
            else
            {
                if (distanceToTarget > detectionRadius)
                {
                    isPatrolling = true;
                    isAttacking = false;
                }
                else
                {
                    Chase();
                }
            }
        }

        if (isAttacking)
        {
            attackCooldownTimer += Time.deltaTime;
            if (attackCooldownTimer >= attackCooldown)
            {
                isAttacking = false;
                attackCooldownTimer = 0.0f;
            }
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length > 0)
        {
            Vector3 targetPoint = patrolPoints[currentPatrolPointIndex].transform.position;
            Vector3 moveDirection = (targetPoint - transform.position).normalized;

            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
                Flip();
            }

            transform.Translate(moveDirection * patrolSpeed * Time.deltaTime);
            animator.SetFloat("Speed", Mathf.Abs(moveDirection.x));
        }
    }

    private void Chase()
    {
        Vector3 moveDirection = (target.transform.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

        if (distanceToTarget <= attackRange)
        {
            Attack();
        }
        else
        {
            isAttacking = false;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            if (moveDirection.x > 0 && isFacingRight)
            {
                Flip();
            }
            else if (moveDirection.x < 0 && !isFacingRight)
            {
                Flip();
            }

            animator.SetFloat("Speed", Mathf.Abs(moveDirection.x));
        }
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
*/