using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public GameObject targetObject; // Kontrol edece�imiz GameObject
    public Health health;

    void Update () 
    { 
        // �arta ba�l� olarak GameObject'i etkinle�tirme veya devre d��� b�rakma �rne�i:
        if (health.currentHealth != 100)
        {
            // GameObject'i etkinle�tirme
            targetObject.SetActive(true);
        }
        else
        {
            // GameObject'i devre d��� b�rakma
            targetObject.SetActive(false);
        }
    }
}
