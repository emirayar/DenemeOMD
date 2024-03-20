using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTutorial : MonoBehaviour
{
    public Transform laserfirepoint;
    public LineRenderer lineRenderer;
    public PlayerHealth currentHealth; // Do�ru de�i�ken ad�

    public float damageInterval = 1f; // Lazerin hasar verme aral��� (�rne�in, her 1 saniyede bir)
    public int laserDamage = 10; // Lazerin verdi�i hasar miktar�

    private float nextDamageTime; // Bir sonraki hasar verme zaman�

    private void Start()
    {
        // nextDamageTime'� ba�lang��ta ayarla
        nextDamageTime = Time.time + damageInterval;

        if (currentHealth == null) // Do�ru de�i�keni kontrol et
        {
            Debug.LogError("PlayerHealth component is not assigned!");
        }
    }

    private void Update()
    {
        if (Time.time >= nextDamageTime)
        {
            ShootLaser();
            nextDamageTime = Time.time + damageInterval; // Bir sonraki hasar� planla
        }
    }

    void ShootLaser()
    {
        RaycastHit2D _hit = Physics2D.Raycast(laserfirepoint.position, transform.right);

        if (_hit)
        {
            Draw2DRay(laserfirepoint.position, _hit.point);
            // �arpan nesne oyuncu ise
            if (_hit.collider.CompareTag("Player"))
            {
                // PlayerHealth scriptindeki TakeDamage metodunu kullanarak oyuncunun sa�l�k de�erini azalt
                currentHealth.TakeDamage(laserDamage); // Lazerin verdi�i hasar miktar�
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
