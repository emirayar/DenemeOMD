using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MonoBehaviour
{
    private CombatController combatControllerScript;
    private LogManager logManager;
    void Start()
    {
        combatControllerScript = GetComponent<CombatController>();
        logManager = GetComponentInChildren<LogManager> ();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Katana"))
        {
            Destroy(other.gameObject);
            logManager.Log("Katana Alýndý");
            combatControllerScript.enabled = true;
        }
    }
}
