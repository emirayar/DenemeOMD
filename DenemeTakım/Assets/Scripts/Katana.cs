using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MonoBehaviour
{
    private CombatPlayer combatPlayerScript;
    private AudioSource _audio;
    public LogManager logManager;
    void Start()
    {
        combatPlayerScript = GetComponent<CombatPlayer>();
        _audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Katana"))
        {
            _audio.Play();
            Destroy(other.gameObject);
            logManager.Log("Katana Alýndý");
            combatPlayerScript.enabled = true;
        }
    }
}
