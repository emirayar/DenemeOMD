using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTutorial : MonoBehaviour
{
    public Transform laserfirepoint;
    private LineRenderer lineRenderer;
    public PlayerHealth currentHealth;

    [SerializeField] private float damageInterval; // Lazerin hasar verme aralýðý (örneðin, her 1 saniyede bir)
    [SerializeField] private int initialLaserDamage; // Lazerin baþlangýçta verdiði hasar miktarý
    private int laserDamage; // Lazerin verdiði hasar miktarý

    private float nextDamageTime; // Bir sonraki hasar verme zamaný

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // nextDamageTime'ý baþlangýçta ayarla
        nextDamageTime = Time.time + damageInterval;

        // Baþlangýç hasarýný ayarla
        laserDamage = initialLaserDamage;
    }

    private void Update()
    {
        if (Time.time >= nextDamageTime)
        {
            ShootLaser();
            nextDamageTime = Time.time + damageInterval; // Bir sonraki hasarý planla
        }
    }

    void ShootLaser()
    {
        RaycastHit2D _hit = Physics2D.Raycast(laserfirepoint.position, transform.right);

        if (_hit)
        {
            Draw2DRay(laserfirepoint.position, _hit.point);
            // Çarpan nesne oyuncu ise
            if (_hit.collider.CompareTag("Player"))
            {
                currentHealth.TakeDamage(laserDamage); 
                laserDamage += 5;
            }else
            {
                laserDamage = initialLaserDamage;
            }
        }
        else
        {
            Draw2DRay(laserfirepoint.position, laserfirepoint.position + laserfirepoint.right * 100);
        }
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
}
