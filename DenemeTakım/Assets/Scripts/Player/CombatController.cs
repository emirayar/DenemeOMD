using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour
{
    public AudioClip[] audioClips;
    private int currentAudioClipsIndex = 0;

    private Animator animator;
    private bool isAttacking = false;
    private int comboCounter = 0;
    private int maxCombo = 2;
    private float attackCooldown = 3f;
    private float timeSinceLastAttack = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        enabled = false;
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
    }

    void AttackSounds()
    {
        AudioSource.PlayClipAtPoint(audioClips[currentAudioClipsIndex], transform.position);

        currentAudioClipsIndex = (currentAudioClipsIndex + 1) % audioClips.Length;
    }

    private IEnumerator PerformCombo()
    {
        isAttacking = true;
        timeSinceLastAttack = 0f;
        animator.SetTrigger(comboCounter + "Attack");
        AttackSounds();

        

        if (comboCounter >= maxCombo)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            // Bekletme süresi boyunca baþka saldýrýya izin verme
            yield return new WaitForSeconds(0);
        }

        // Bekletme süresi sonrasýnda combo sýfýrla
        isAttacking = false;

    }
}
