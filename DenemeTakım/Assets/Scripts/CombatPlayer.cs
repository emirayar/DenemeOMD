using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPlayer : MonoBehaviour
{
    public AudioClip[] audioClips;
    private int currentAudioClipsIndex = 0;

    public Animator animator;
    public float comboTime = 0.7f;

    private float comboTimer;
    private bool isComboActive = false;

    void Start()
    {
        enabled = false;
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (isComboActive)
            {
                Attack2();
                AttackSounds();
            }
            else
            {
                Attack();
                AttackSounds();
            }
        }

        if (isComboActive)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }

    }
        void Attack()
        {
            animator.SetTrigger("Attack");

            // Combo zamanlayıcısını başlatın ve combo'yu etkin olarak işaretleyin.
            comboTimer = comboTime;
            isComboActive = true;
            animator.SetBool("ComboTime", true);
        }

        void Attack2()
        {
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

        void AttackSounds()
        {
            if (audioClips.Length > 0)
            {
                AudioSource.PlayClipAtPoint(audioClips[currentAudioClipsIndex], transform.position);

                currentAudioClipsIndex = (currentAudioClipsIndex + 1) % audioClips.Length;
            }
        }
    }

