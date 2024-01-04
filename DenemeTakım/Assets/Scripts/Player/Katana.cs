using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MonoBehaviour
{
    private CombatController combatControllerScript;
    public LogManager logManager;
    public AudioClip getsound;
    void Start()
    {
        combatControllerScript = GetComponent<CombatController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Katana"))
        {
            AudioSource.PlayClipAtPoint(getsound, other.transform.position);
            Destroy(other.gameObject);
            logManager.Log("Katana Alýndý");
            combatControllerScript.enabled = true;
        }
    }
}
