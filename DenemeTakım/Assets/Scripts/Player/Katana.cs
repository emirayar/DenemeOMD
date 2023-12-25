using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MonoBehaviour
{
    private CombatPlayer combatPlayerScript;
    public LogManager logManager;
    public AudioClip getsound;
    void Start()
    {
        combatPlayerScript = GetComponent<CombatPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Katana"))
        {
            AudioSource.PlayClipAtPoint(getsound, other.transform.position);
            Destroy(other.gameObject);
            logManager.Log("Katana Alýndý");
            combatPlayerScript.enabled = true;
        }
    }
}
