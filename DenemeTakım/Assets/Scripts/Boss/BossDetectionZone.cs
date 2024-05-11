using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDetectionZone : MonoBehaviour
{
    private Boss boss;
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Boss!");
            boss.inZone = true;
            Destroy(gameObject);
        }
    }
}
