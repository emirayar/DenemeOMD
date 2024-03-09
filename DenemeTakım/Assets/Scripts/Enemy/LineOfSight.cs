using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;

            // Düþmanýn bakýþ açýsýný ve hedefin pozisyonunu dikkate alarak açýyý hesapla
            float rightAngle = Vector2.Dot(transform.right, dirToTarget);
            float leftAngle = Vector2.Dot(-transform.right, dirToTarget);

            // Eðer hedef düþmanýn önünde ise
            if (rightAngle > 0 || leftAngle > 0)
            {
                // Eðer hedefin açýsý, düþmanýn bakýþ açýsýnýn yarýsý kadarlýk açýdan küçükse veya eþitse, hedef düþmanýn görüþ alanýndadýr.
                float dstToTarget = Vector2.Distance(transform.position, target.position);

                // Engellere çarpýp çarpmadýðýný kontrol et
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        foreach (Transform visibleTarget in visibleTargets)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, visibleTarget.position);
        }
    }
}