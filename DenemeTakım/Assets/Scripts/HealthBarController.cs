using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public GameObject targetObject; // Kontrol edeceðimiz GameObject
    public Health health;

    void Update () 
    { 
        // Þarta baðlý olarak GameObject'i etkinleþtirme veya devre dýþý býrakma örneði:
        if (health.currentHealth != 100)
        {
            // GameObject'i etkinleþtirme
            targetObject.SetActive(true);
        }
        else
        {
            // GameObject'i devre dýþý býrakma
            targetObject.SetActive(false);
        }
    }
}
