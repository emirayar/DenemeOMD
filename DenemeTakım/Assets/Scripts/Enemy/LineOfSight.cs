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

            // D��man�n bak�� a��s�n� ve hedefin pozisyonunu dikkate alarak a��y� hesapla
            float rightAngle = Vector2.Dot(transform.right, dirToTarget);
            float leftAngle = Vector2.Dot(-transform.right, dirToTarget);

            // E�er hedef d��man�n �n�nde ise
            if (rightAngle > 0 || leftAngle > 0)
            {
                // E�er hedefin a��s�, d��man�n bak�� a��s�n�n yar�s� kadarl�k a��dan k���kse veya e�itse, hedef d��man�n g�r�� alan�ndad�r.
                float dstToTarget = Vector2.Distance(transform.position, target.position);

                // Engellere �arp�p �arpmad���n� kontrol et
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