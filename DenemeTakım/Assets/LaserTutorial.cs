using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTutorial : MonoBehaviour
{
    public Transform laserfirepoint;
    public LineRenderer lineRenderer;
    public PlayerHealth currentHealth; // Doðru deðiþken adý

    public float damageInterval = 1f; // Lazerin hasar verme aralýðý (örneðin, her 1 saniyede bir)
    public int laserDamage = 10; // Lazerin verdiði hasar miktarý

    private float nextDamageTime; // Bir sonraki hasar verme zamaný

    private void Start()
    {
        // nextDamageTime'ý baþlangýçta ayarla
        nextDamageTime = Time.time + damageInterval;

        if (currentHealth == null) // Doðru deðiþkeni kontrol et
        {
            Debug.LogError("PlayerHealth component is not assigned!");
        }
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
                // PlayerHealth scriptindeki TakeDamage metodunu kullanarak oyuncunun saðlýk deðerini azalt
                currentHealth.TakeDamage(laserDamage); // Lazerin verdiði hasar miktarý
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
