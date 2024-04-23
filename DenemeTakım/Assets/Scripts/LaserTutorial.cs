using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTutorial : MonoBehaviour
{
    public Transform laserfirepoint;
    private LineRenderer lineRenderer;

    [SerializeField] private float damageInterval;
    [SerializeField] private int initialLaserDamage;
    private int laserDamage;

    private float nextDamageTime;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        nextDamageTime = Time.time + damageInterval;
        laserDamage = initialLaserDamage;
    }

    private void Update()
    {
        if (Time.time >= nextDamageTime)
        {
            ShootLaser();
            nextDamageTime = Time.time + damageInterval;
        }
    }

    void ShootLaser()
    {
        RaycastHit2D _hit = Physics2D.Raycast(laserfirepoint.position, transform.right);

        if (_hit)
        {
            Draw2DRay(laserfirepoint.position, _hit.point);
            if (_hit.collider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = _hit.collider.GetComponent<PlayerHealth>();
                if(playerHealth != null)
                {
                    playerHealth.TakeDamage(laserDamage);
                    laserDamage += 5;
                }
            }
            else if(_hit.collider.CompareTag("Enemy"))
            {
                Health health = _hit.collider.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(laserDamage);
                    laserDamage += 5;
                }
            }
        }
        else
        {
            Draw2DRay(laserfirepoint.position, laserfirepoint.position + laserfirepoint.right * 100);
            laserDamage = initialLaserDamage;
        }
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
}