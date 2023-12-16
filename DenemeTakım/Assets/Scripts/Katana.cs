using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MonoBehaviour
{
    public CombatPlayer combatPlayerScript;
    private AudioSource audio;
    void FixedUpdate()
    {
        combatPlayerScript = GetComponent<CombatPlayer>();
    }
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Katana"))
        {
            audio.Play();
            Destroy(other.gameObject);
            Debug.Log("Katana Alýndý");
            combatPlayerScript.enabled = true;
        }
    }
}
