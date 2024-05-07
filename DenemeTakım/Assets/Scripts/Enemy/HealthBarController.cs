using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public GameObject targetObject;
    public Health health;
    private int detectRadius = 3;
    [SerializeField] private LayerMask layerMask;

    void Update()
    {
        Detector();
        OpenClose();
    }

    void Detector()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectRadius, layerMask);

        if (colliders.Length > 0)
        {
            health.isDetected = true;
        }
        else
        {
            health.isDetected = false;
        }
    }
    void OpenClose()
    {
        if (health.currentHealth != 100 || health.isDetected)
        {
            targetObject.SetActive(true);
        }
        else
        {
            targetObject.SetActive(false);
        }
    }
}